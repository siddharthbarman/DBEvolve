namespace SByteStream.DBEvolve
{
    public class ScriptFile
    {
        public ScriptFile(int version, string filePath, byte[] fileHash)
        {
            Version = version;
            FilePath = filePath;
            FileHash = fileHash;
        }

        public int Version { get; set; }
        
        public string FilePath { get; set; }

        public byte[] FileHash { get; set; }

        public override string ToString()
        {
            return FilePath;
        }
    }
}
