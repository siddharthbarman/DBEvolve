using Microsoft.Extensions.Logging;
using SByteStream.DBEvolve;

public class Program
{
    static void TestPostgres(ILogger logger)
    {        
        string connectionString = "Server=localhost; Port=5432; Database=PackageTestDb; UserId=postgres; Password=password123;";
        string scriptsDirectory = @".\DbScripts\Postgres";
        var evolver = new PostgresDBEvolver();
        evolver.Evolve(logger, connectionString, scriptsDirectory);
    }

    static void TestMySql(ILogger logger)
    {
        string connectionString = "server=localhost; port=3306; uid=root; pwd=password123; database=PackageTestDb";
        string scriptsDirectory = @".\DbScripts\MySql";
        var evolver = new MySqlDBEvolver();
        evolver.Evolve(logger, connectionString, scriptsDirectory);
    }

    static void TestSqlServer(ILogger logger)
    {
        string connectionString = "Data Source=localhost; Initial Catalog=PackageTestDb; Integrated Security=True;";
        string scriptsDirectory = @".\DbScripts\SqlServer";
        var evolver = new SqlServerDBEvolver();
        evolver.Evolve(logger, connectionString, scriptsDirectory);
    }

    static void Main()
    {
        // Create a logger (using any ILogger implementation) 
        ILogger logger = LoggerFactory.Create(builder => builder.AddConsole())
            .CreateLogger("DBEvolve");

        try
        {
            TestPostgres(logger);
            Console.WriteLine("Postgres test suceeded.");
        }
        catch(Exception e)
        {
            Console.WriteLine("Postgres test failed.");
            Console.WriteLine(e.Message);
        }

        try
        {
            TestMySql(logger);
            Console.WriteLine("MySql test suceeded.");
        }
        catch (Exception e)
        {
            Console.WriteLine("MySql test failed.");
            Console.WriteLine(e.Message);
        }

        try
        {
            TestSqlServer(logger);
            Console.WriteLine("SqlServer test suceeded.");
        }
        catch (Exception e)
        {
            Console.WriteLine("SqlServer test failed.");
            Console.WriteLine(e.Message);
        }
    }
}




