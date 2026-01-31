using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;

namespace SByteStream.DBEvolve
{
    public class SqlServerDbManager : DbManagerBase
    {
        public SqlServerDbManager(ILogger logger) : base(logger)
        {
        }

        public override void CreateDbConnection()
        {
            Logger?.LogInformation("Creating database connection.");
            m_conn = new SqlConnection(ConnectionString);
            m_conn.Open();
            Logger?.LogInformation("Database connection created successfully.");
        }

        public override void CreateVersionHistoryTable()
        {
            Logger?.LogInformation($"Checking existence of {m_versionHistoryTableName} table.");
            using (SqlCommand cmd = CreateSqlCommand("select count(1) from sys.tables where name = @tableName", m_conn!))
            {
                cmd.Parameters.AddWithValue("@tableName", m_versionHistoryTableName);
                int result = (int)cmd.ExecuteScalar();
                if (result == 0)
                {
                    Logger?.LogInformation($"{m_versionHistoryTableName} table does not exist, will create it.");
                    string createTableSql = string.Format(VERSION_HISTORY_TABLE_SCRIPT, m_versionHistoryTableName);
                    using (SqlCommand createCmd = CreateSqlCommand(createTableSql, m_conn!))
                    {
                        createCmd.ExecuteNonQuery();
                    }
                    Logger?.LogInformation($"{m_versionHistoryTableName} table created successfully.");
                }
            }
        }

        public override List<ScriptFile> GetScriptEntries()
        {
            Logger?.LogInformation("Retrieving script entries from version history table.");

            List<ScriptFile> scriptFiles = new List<ScriptFile>();
            
            using (SqlCommand cmd = CreateSqlCommand($"select VersionNumber, Filename, FileHash from [{m_versionHistoryTableName}] order by VersionNumber", 
                m_conn!))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int version = reader.GetInt32(0);
                        string fileName = reader.GetString(1);
                        byte[] fileHash = (byte[])reader.GetValue(2);
                        ScriptFile scriptFile = new ScriptFile(version, fileName, fileHash);
                        scriptFiles.Add(scriptFile);
                    }
                }
            }
            
            Logger?.LogInformation("Retrieved {0} script entries from version history table.", scriptFiles.Count);
            return scriptFiles;
        }

        public override void CreateDatabaseIfNotExists()
        {
            Logger?.LogInformation("Attemping to create target database if not present.");

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConnectionString);
            builder.ConnectionString = ConnectionString;
            string databaseName = builder.InitialCatalog;
            builder.InitialCatalog = "master";

            using (SqlConnection conn = new SqlConnection(builder.ToString()))
            {
                conn.Open();
                if (!CheckDbExists(conn, databaseName))
                {
                    Logger?.LogInformation("Creating target database: {0}.", databaseName);
                    using (SqlCommand cmd = CreateSqlCommand($"CREATE DATABASE [{databaseName}]", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    Logger?.LogInformation("Target database: {0} created successfully.", databaseName);
                }
            }
        }

        public override void RunScript(string scriptFile)
        {
            Logger?.LogInformation("Running script: {0}.", scriptFile);

            StringBuilder sb = new StringBuilder();
            string? line;
            bool firstLine = true;

            using (StreamReader sr = new StreamReader(File.OpenRead(scriptFile)))
            {
                line = sr.ReadLine()?.Trim();
                if (firstLine)
                {
                    if (line?.Trim() == NO_TRANSACTION_INDICATOR)
                    {
                        Logger?.LogWarning("Script has instructed to turn off transaction.");
                        m_tran = null;
                        line = sr.ReadLine()?.Trim();
                    }
                    else
                    {
                        m_tran = m_conn?.BeginTransaction();
                    }
                    firstLine = false;
                }

                while (line != null)
                {
                    if (line.ToLower() == "go")
                    {
                        RunSql(sb.ToString());
                        sb.Clear();
                    }
                    else
                    {
                        sb.AppendLine(line);
                    }
                    line = sr.ReadLine()?.Trim();
                }

                if (sb.Length > 0)
                {
                    string sql = sb.ToString().Trim();
                    RunSql(sql);
                    sb.Clear();
                }
            }            

            Logger?.LogInformation("Finished running script: {0}.", scriptFile);
        }

        public override void CommitScript()
        {
            if (m_tran != null)
            {
                m_tran?.Commit();
            }
        }

        public override void RollbackScript()
        {
            m_tran?.Rollback();
        }

        public override void MakeScriptEntry(ScriptFile scriptFile)
        {
            Logger?.LogInformation("Making entry for script: {0}.", scriptFile);

            string insertSql = string.Format(INSERT_SCRIPT_ENTRY_FMT, m_versionHistoryTableName);
            
            using (SqlCommand cmd = CreateSqlCommand(insertSql, m_conn!))
            {
                if (m_tran != null)
                {
                    cmd.Transaction = m_tran;
                }
                cmd.Parameters.AddWithValue("@versionNo", scriptFile.Version);
                cmd.Parameters.AddWithValue("@fileName", scriptFile.FilePath);
                cmd.Parameters.AddWithValue("@fileHash", scriptFile.FileHash);
                cmd.ExecuteNonQuery();
            }

            Logger?.LogInformation("Entry created for script: {0}.", scriptFile);
        }

        public override void RunSql(string sql)
        {
            Logger?.LogDebug("Executing SQL: {0}", sql);

            if (string.IsNullOrEmpty(sql))
            {
                return;
            }

            try
            {
                using (SqlCommand cmd = CreateSqlCommand(sql, m_conn!))
                {
                    if (m_tran != null)
                    {
                        cmd.Transaction = m_tran;
                    }
                    cmd.ExecuteNonQuery();
                }
            }
            catch
            {
                Logger?.LogError("Error executing SQL: {0}.", sql);
                throw;
            }
        }

        public override int GetCurrentDbVersion()
        {
            Logger?.LogInformation("Retrieving schema version.");

            int version;

            using (SqlCommand cmd = CreateSqlCommand($"select max(VersionNumber) from [{m_versionHistoryTableName}]", 
                m_conn!))
            {
                object result = cmd.ExecuteScalar();
                version = (result == DBNull.Value) ? 0 : (int)result;                
            }

            Logger?.LogInformation("Retrieved schema version: {0}.", version);
            return version;
        }

        public override void Dispose()
        {
            if (m_conn != null)
            {
                m_conn.Close();
                m_conn.Dispose();
                m_conn = null;
            }
        }

        private bool CheckDbExists(SqlConnection conn, string dbName)
        {
            using (SqlCommand cmd = CreateSqlCommand($"select count(1) from sys.databases where name = @dbName", conn))
            {            
                cmd.Parameters.AddWithValue("@dbName", dbName);
                int result = (int)cmd.ExecuteScalar();
                return result > 0;
            }
        }

        private SqlCommand CreateSqlCommand(string sqlText, SqlConnection conn)
        {
            return new SqlCommand(sqlText, conn)
            {
                CommandTimeout = CommandTimeoutSec
            };
        }
        
        private const string VERSION_HISTORY_TABLE_SCRIPT = @"create table [{0}]
        (
	        VersionNumber int not null primary key,
	        Filename nvarchar(512) not null,
            FileHash varbinary(32) not null,
	        EntryDate datetime constraint [DF_{0}_EntryDate] default (getutcdate())
        )";

        private const string INSERT_SCRIPT_ENTRY_FMT = "insert into [{0}](VersionNumber, Filename, FileHash) values(@versionNo, @fileName, @fileHash);";


        private SqlConnection? m_conn;
        private SqlTransaction? m_tran;
    }
}
