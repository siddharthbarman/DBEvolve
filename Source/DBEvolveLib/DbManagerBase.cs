using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace SByteStream.DBEvolve
{
    public abstract class DbManagerBase : IDbManager
    {
        public DbManagerBase(ILogger? logger)
        {
            Logger = logger;
        }

        public virtual void Initialize(string connectionString, string versionHistoryTableName, int commandTimeoutSec)
        {
            ConnectionString = connectionString;
            m_versionHistoryTableName = versionHistoryTableName;
            m_commandTimeoutSec = commandTimeoutSec;
            CreateDatabaseIfNotExists();
            CreateDbConnection();
            CreateVersionHistoryTable();            
        }

        public string? ConnectionString { get; set; }
        
        public abstract void CreateDatabaseIfNotExists();

        public abstract void CreateDbConnection();

        public abstract void CreateVersionHistoryTable();

        public abstract List<ScriptFile> GetScriptEntries();

        public abstract void RunScript(string scriptFile);

        public abstract void MakeScriptEntry(ScriptFile scriptFile);

        public abstract void RunSql(string sql);

        public abstract int GetCurrentDbVersion();
        
        public void ValidateScripts(string scriptsDirectory)
        {
            List<ScriptFile> scriptFilesInDb = GetScriptEntries();
            List<ScriptFile> localScripts = GetDbScripts(scriptsDirectory);
            foreach(ScriptFile script in scriptFilesInDb)
            {
                ScriptFile localScript = localScripts.FirstOrDefault(s => s.FilePath == script.FilePath);
                if (localScript == null)
                {
                    throw new ScriptNotFoundException($"Script file {script.FilePath} recorded in database does not exist in scripts directory.");
                }

                if (!File.Exists(script.FilePath))
                {
                    throw new ScriptNotFoundException($"Script file {script.FilePath} recorded in database does not exist in scripts directory.");
                }

                byte[] localFileHash = GetFileHash(localScript.FilePath);
                if (!localFileHash.SequenceEqual(script.FileHash))
                {
                    throw new ScriptModifiedException($"Script file {script.FilePath} has been modified since it was applied to the database.");
                }                
            }
        }

        protected List<ScriptFile> GetDbScripts(string scriptsDirectory)
        {
            List<string> files = Directory.GetFiles(scriptsDirectory, "*.sql").ToList();
            List<ScriptFile> scriptFiles = new List<ScriptFile>();

            foreach (string file in files)
            {
                Utils.ParseScriptFilename(file, out int version);
                ScriptFile scriptFile = new ScriptFile(version, file, GetFileHash(file));
                scriptFiles.Add(scriptFile);
            }

            return scriptFiles.OrderBy(sf => sf.Version).ToList();
        }

        public void RunScripts(string scriptsDirectory, int maxVersion)
        {
            List<ScriptFile> scripts = GetDbScripts(scriptsDirectory);
            int currentDbVersion = GetCurrentDbVersion();

            foreach (ScriptFile script in scripts)
            {
                if (script.Version > currentDbVersion && (maxVersion == 0 || script.Version <= maxVersion))
                {
                    try
                    {
                        RunScript(script.FilePath);
                        MakeScriptEntry(script);
                        CommitScript();
                    }
                    catch
                    {
                        RollbackScript();
                        throw;
                    }
                }
                else
                {
                    Logger?.LogInformation("Skipping script: {0} as its version {1} is not in the upgrade range.", script.FilePath, script.Version);
                }
            }
        }

        public abstract void CommitScript();

        public abstract void RollbackScript();

        public byte[] GetFileHash(string filePath)
        {
            using var sha256 = SHA256.Create();
            using var stream = File.OpenRead(filePath);
            return sha256.ComputeHash(stream);
        }

        public ILogger? Logger 
        { 
            get { return m_logger; }
            private set { m_logger = value; }
        }

        public int CommandTimeoutSec
        {
            get { return m_commandTimeoutSec; }
            private set { m_commandTimeoutSec = value; }
        }

        public abstract void Dispose();
        
        private ILogger? m_logger;
        private int m_commandTimeoutSec;
        protected string? m_versionHistoryTableName;
        protected const string NO_TRANSACTION_INDICATOR = "-- DBEVOLVE: NO_TRANSACTION";
    }
}
