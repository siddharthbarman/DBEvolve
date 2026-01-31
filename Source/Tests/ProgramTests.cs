using Microsoft.Extensions.Logging;

namespace SByteStream.DBEvolve
{
    public class ProgramTests
    {
        [Test]
        public void Test_Method_Call_Sequence()
        {
            DbManagerBase dbManager = new TestDbManager(new SimpleFileLogger("Tests.log", LogLevel.Trace));
            dbManager.Initialize("Server=.;Database=TestDb;User Id=sa;Password=your_password;", "VERSION", 30);
            List<string> expectedSequence = new List<string>
            {
                "CreateDatabaseIfNotExists",
                "CreateDbConnection",
                "CreateVersionHistoryTable"
            };

            TestDbManager testDbManager = (TestDbManager)dbManager;
            Assert.That(testDbManager.MethodCallList[0], Is.EqualTo("CreateDatabaseIfNotExists"));
            Assert.That(testDbManager.MethodCallList[1], Is.EqualTo("CreateDbConnection"));
            Assert.That(testDbManager.MethodCallList[2], Is.EqualTo("CreateVersionHistoryTable"));
        }

        [Test]
        public void Given_Version_Zero_All_Scripts_Are_Run()
        {
            DbManagerBase dbManager = new TestDbManager(new SimpleFileLogger("Tests.log", LogLevel.Trace));
            dbManager.Initialize("Server=.;Database=TestDb;User Id=sa;Password=your_password;", "VERSION", 30);
            dbManager.RunScripts("TestScripts", 0);

            TestDbManager testDbManager = (TestDbManager)dbManager;            
            Assert.That(testDbManager.SqlScriptsRun[0], Is.EqualTo("TestScripts\\V01_00__initial_schema.sql"));
            Assert.That(testDbManager.SqlScriptsRun[1], Is.EqualTo("TestScripts\\V01_01__view.sql"));
        }

        [Test]
        public void Given_Existing_Version_Higher_Version_Scripts_Are_Run()
        {
            TestDbManager dbManager = new TestDbManager(new SimpleFileLogger("Tests.log", LogLevel.Trace));
            dbManager.CurrentVersion = 100;
            dbManager.Initialize("Server=.;Database=TestDb;User Id=sa;Password=your_password;", "VERSION", 30);
            dbManager.RunScripts("TestScripts", 0);
            Assert.That(dbManager.SqlScriptsRun[0], Is.EqualTo("TestScripts\\V01_01__view.sql"));
        }

        [Test]
        public void Given_Max_Version_LowerAndEquals_Version_Scripts_Are_Run()
        {
            TestDbManager dbManager = new TestDbManager(new SimpleFileLogger("Tests.log", LogLevel.Trace));
            dbManager.CurrentVersion = 0;
            dbManager.Initialize("Server=.;Database=TestDb;User Id=sa;Password=your_password;", "VERSION", 30);
            dbManager.RunScripts("TestScripts", 100);
            Assert.That(dbManager.SqlScriptsRun[0], Is.EqualTo("TestScripts\\V01_00__initial_schema.sql"));
            Assert.That(dbManager.SqlScriptsRun.Count, Is.EqualTo(1));
        }
    }
}
