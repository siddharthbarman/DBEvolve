using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace SByteStream.DBEvolve
{
    public class DBEvolver
    {
        public void Evolve(ILogger logger, string connectionString, string scriptsDirectory, 
            string versionTableName = "__Version_History__", int maxVersion = 0, int commandTimeoutSec = 30)
        {
            using (IDbManager mgr = new SqlServerDbManager(logger))
            {
                mgr.Initialize(connectionString!, versionTableName, commandTimeoutSec);
                mgr.ValidateScripts(scriptsDirectory);
                mgr.RunScripts(scriptsDirectory, maxVersion);
            }
        }
    }
}
