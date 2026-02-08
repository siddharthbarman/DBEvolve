using Microsoft.Extensions.Logging;

namespace SByteStream.DBEvolve
{
    public class MySqlDBEvolver
    {
        public void Evolve(ILogger logger, string connectionString, string scriptsDirectory,
            string versionTableName = "__Version_History__", int maxVersion = 0, int commandTimeoutSec = 30)
        {
            using (IDbManager mgr = new MySqlDbManager(logger))
            {
                mgr.Initialize(connectionString!, versionTableName, commandTimeoutSec);
                mgr.ValidateScripts(scriptsDirectory);
                mgr.RunScripts(scriptsDirectory, maxVersion);
            }
        }
    }
}
