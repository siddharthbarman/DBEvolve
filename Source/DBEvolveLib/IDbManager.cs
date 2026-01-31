using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace SByteStream.DBEvolve
{
    public interface IDbManager : IDisposable
    {
        void Initialize(string connectionString,  string versionHistoryTableName, int commandTimeoutSec);

        void CreateDatabaseIfNotExists();

        void CreateDbConnection();

        void CreateVersionHistoryTable();

        List<ScriptFile> GetScriptEntries();

        void RunScripts(string scriptsDirectory, int maxVersion);

        void RunScript(string scriptFile);

        void CommitScript();

        void RollbackScript();

        void RunSql(string sql);

        void MakeScriptEntry(ScriptFile scriptFile);

        int GetCurrentDbVersion();

        void ValidateScripts(string scriptsDirectory);

        byte[] GetFileHash(string filePath);

        ILogger? Logger { get; }

        int CommandTimeoutSec { get; }
    }
}
