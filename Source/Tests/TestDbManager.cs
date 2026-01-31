using Microsoft.Extensions.Logging;
using System.Data.SqlClient;

namespace SByteStream.DBEvolve
{
    public class TestDbManager : DbManagerBase
    {
        public TestDbManager(ILogger logger) : base(logger)
        {
        }

        public List<string> MethodCallList = new List<string>();
        public List<string> SqlScriptsRun = new List<string>();

        public override void CreateDbConnection()
        {
            MethodCallList.Add("CreateDbConnection");
        }

        public override void CreateVersionHistoryTable()
        {
            MethodCallList.Add("CreateVersionHistoryTable");
        }

        public override List<ScriptFile> GetScriptEntries()
        {
            return new List<ScriptFile>();
        }

        public override void CreateDatabaseIfNotExists()
        {
            MethodCallList.Add("CreateDatabaseIfNotExists");
        }

        public override void RunScript(string scriptFile)
        {
            SqlScriptsRun.Add(scriptFile);
        }

        public override void CommitScript()
        {            
        }

        public override void RollbackScript()
        {            
        }

        public override void MakeScriptEntry(ScriptFile scriptFile)
        {
        }

        public override void RunSql(string sql)
        {            
        }

        public override int GetCurrentDbVersion()
        {
            return CurrentVersion;
        }

        public override void Dispose()
        {            
        }

        public int CurrentVersion { get; set; } = 0;

        private bool CheckDbExists(SqlConnection conn, string dbName)
        {
            return true;
        }        
    }
}
