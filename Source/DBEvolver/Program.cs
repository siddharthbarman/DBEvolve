using Microsoft.Extensions.Logging;

namespace SByteStream.DBEvolve
{
    internal enum DatabaseTypes
    {    
        SqlServer,
        Postgres,
        MySql
    }

    internal class Program
    {
        static void Help()
        {
            Console.WriteLine("DBEvoler is a database maintenance utility which can do the following:");
            Console.WriteLine("1. Create a database");
            Console.WriteLine("2. Migrate a database to the latest version");
            Console.WriteLine();
            Console.WriteLine("Syntax:");
            Console.WriteLine("  dbevolver -t <database type> -c <connection string> -f <scripts folder> -v <version> -n <version tablename>");
            Console.WriteLine();
            Console.WriteLine("Parameters:");
            Console.WriteLine("  -t: Valid values: SqlServer, Postgres, MySql.");
            Console.WriteLine("  -c: Connection string including the database name.");
            Console.WriteLine("  -f: Specifes the folder where database scripts are located. Optional. Default value is .\\dbscripts");
            Console.WriteLine("  -v: Specifies the version till which to upgrade the database. Optional. Default is to upgrade to the maximum version.");
            Console.WriteLine("  -n: Specifies the name of the version table. Optional. Default is __Version_History__");
            Console.WriteLine();
            
            Console.WriteLine("SqlServer Examples:");
            string connStr = "\"Data Source=localhost; Initial Catalog=MyAppDb; User Id=TestUser; Password=password123; Integrated Security=False;\"";
            Console.WriteLine("  dbevolver -t SqlServer -c {0}", connStr);
            Console.WriteLine("  dbevolver -t SqlServer -c {0} -n SchemaVersions", connStr);
            Console.WriteLine("  dbevolver -t SqlServer -c {0} -f .\\scripts", connStr);
            Console.WriteLine("  dbevolver -t SqlServer -c {0} -v 101", connStr);
            Console.WriteLine("  dbevolver -t SqlServer -c {0} -f .\\scripts", connStr);
            Console.WriteLine("  dbevolver -t SqlServer -c {0} -v 101 -f .\\scripts", connStr);
            
            Console.WriteLine("Postgres Examples:");
            connStr = "\"Server=localhost; Port=5432; Database=MyAppDb; UserId=postgres; Password=password123;\"";
            Console.WriteLine("  dbevolver -t Postgres -c {0}", connStr);
            Console.WriteLine("  dbevolver -t Postgres -c {0} -n SchemaVersions", connStr);
            Console.WriteLine("  dbevolver -t Postgres -c {0} -f .\\scripts", connStr);
            Console.WriteLine("  dbevolver -t Postgres -c {0} -v 101", connStr);
            Console.WriteLine("  dbevolver -t Postgres -c {0} -f .\\scripts", connStr);
            Console.WriteLine("  dbevolver -t Postgres -c {0} -v 101 -f .\\scripts", connStr);

            Console.WriteLine("MySql Examples:");
            connStr = "\"server=localhost; port=3306; uid=root; pwd=password123; database=MyAppDb\"";
            Console.WriteLine("  dbevolver -t MySql -c {0}", connStr);
            Console.WriteLine("  dbevolver -t MySql -c {0} -n SchemaVersions", connStr);
            Console.WriteLine("  dbevolver -t MySql -c {0} -f .\\scripts", connStr);
            Console.WriteLine("  dbevolver -t MySql -c {0} -v 101", connStr);
            Console.WriteLine("  dbevolver -t MySql -c {0} -f .\\scripts", connStr);
            Console.WriteLine("  dbevolver -t MySql -c {0} -v 101 -f .\\scripts", connStr);
        }

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Help();
                return;
            }

            if (args.Length < 2)
            {
                logger.Log(LogLevel.Error, "Insufficient arguments.");
                Help();
                return;
            }

            try
            {
                string? connectionString;
                string? scriptsDirectory;
                string dbType;
                int maxVersion = 0;
                string versionTable;
                int commandTimeoutSec;

                GetOptions(args, out dbType, out connectionString, out scriptsDirectory, out maxVersion, 
                    out versionTable, out commandTimeoutSec);

                if (connectionString == null)
                {
                    logger.Log(LogLevel.Error, "Connection string not specified.");
                }

                switch (dbType)
                {
                    case "SqlServer":
                        new SqlServerDBEvolver().Evolve(logger, connectionString!, scriptsDirectory, versionTable,
                            maxVersion, commandTimeoutSec);
                        break;

                    case "Postgres":
                        new PostgresDBEvolver().Evolve(logger, connectionString!, scriptsDirectory, versionTable,
                            maxVersion, commandTimeoutSec);
                        break;

                    case "MySql":
                        new MySqlDBEvolver().Evolve(logger, connectionString!, scriptsDirectory, versionTable,
                            maxVersion, commandTimeoutSec);
                        break;

                    default:
                        throw new DBEvolveException($"{dbType} is an unsupported database type.");
                }
                
                Console.WriteLine("Database evolution completed successfully.");
            }
            catch (DBEvolveException e)
            {
                logger.LogError("DBEvolve error: {0}", e.Message);
            }
            catch (Exception ex)
            {
                logger.LogError("Error running dbevolver: {0}", ex);
            }
        }

        static void GetOptions(string[] args, out string dbTypeName, out string? connectionString, out string scriptsDirectory, 
            out int maxVersion, out string versionTable, out int commandTimeoutSec)
        {
            dbTypeName = string.Empty;
            scriptsDirectory = "dbscripts";
            connectionString = null;
            maxVersion = 0;
            versionTable = "__Version_History__";
            commandTimeoutSec = 30;

            for (int n = 0; n < args.Length; n += 2)
            {
                if (args[n] == "-t")
                {
                    if ((n + 1) >= args.Length)
                    {
                        throw new ArgumentException("Database type not specified.");
                    }

                    dbTypeName = args[n + 1];
                    
                    if (dbTypeName != "SqlServer" && dbTypeName != "Postgres" && dbTypeName != "MySql")
                    {
                        throw new ArgumentException($"{dbTypeName} is not a valid database type.");
                    }
                    
                }
                else if (args[n] == "-c")
                {
                    if ((n + 1) >= args.Length)
                    {
                        throw new ArgumentException("Connection string not specified.");
                    }
                    connectionString = args[n + 1];
                }
                else if (args[n] == "-f")
                {
                    if ((n + 1) >= args.Length)
                    {
                        throw new ArgumentException("Script folder not specified.");
                    }
                    scriptsDirectory = args[n + 1];
                }
                else if (args[n] == "-v")
                {
                    if ((n + 1) >= args.Length)
                    {
                        throw new ArgumentException("Version not specified.");
                    }

                    if (!int.TryParse(args[n + 1], out maxVersion))
                    {
                         throw new ArgumentException($"Invalid version specified: {args[n + 1]}");
                    }                    
                }
                else if (args[n] == "-n")
                {
                    if ((n + 1) >= args.Length)
                    {
                        throw new ArgumentException("Version table not specified.");
                    }
                    versionTable = args[n + 1];
                }
                else if (args[n] == "-t")
                {
                    if ((n + 1) >= args.Length)
                    {
                        throw new ArgumentException("Command timeout not specified.");
                    }

                    if (!int.TryParse(args[n + 1], out commandTimeoutSec))
                    {
                        throw new ArgumentException($"Invalid command timeout specified: {args[n + 1]}");
                    }
                }
                else
                {
                    throw new ArgumentException($"Unknown argument: {args[n]}");
                }
            }
        }

        static ILogger logger = new SimpleFileLogger("DBEvoler", LogLevel.Information);
    }
}
