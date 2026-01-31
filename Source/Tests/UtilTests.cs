namespace SByteStream.DBEvolve
{
    public class UtilTests
    {
        [Test]
        public void Throws_Exception_Given_ScriptFilenameNotBeginWithV()
        {
            Assert.Throws<ApplicationException>(() =>
            {
                Utils.ParseScriptFilename("1_0__InitialSetup.sql", out int version);
            });            
        }

        [Test]
        public void Throws_Exception_Given_ScriptFilenameInvalidFormat()
        {
            Assert.Throws<ApplicationException>(() =>
            {
                Utils.ParseScriptFilename("V1-0__InitialSetup.sql", out int version);
            });

            Assert.Throws<ApplicationException>(() =>
            {
                Utils.ParseScriptFilename("V1__0__InitialSetup.sql", out int version);
            });
        }

        [Test]
        public void Returns_Correct_ScriptFile_Given_ValidFilename()
        {
            Utils.ParseScriptFilename("V1_0__InitialSetup.sql", out int version);
            Assert.That(version, Is.EqualTo(100));            
        }

        [Test]
        public void Returns_Correct_ScriptFile_Given_ValidFilename_With_Preceeding_Zeroes()
        {
            Utils.ParseScriptFilename("V002_5__AddNewTable.sql", out int version);
            Assert.That(version, Is.EqualTo(205));            
        }
    }
}