using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SByteStream.DBEvolve
{
    public class Utils
    {
        public static void ParseScriptFilename(string filePath, out int version)
        {
            string fileName = Path.GetFileName(filePath);

            if (!fileName.StartsWith("V"))
            {
                throw new ApplicationException($"Invalid script file name: {filePath}. Expected to start with 'V'");
            }

            string[] parts = fileName.Split(new string[] { "__" }, System.StringSplitOptions.None);
            if (parts.Length != 2)
            {
                throw new ApplicationException($"Invalid script file name: {filePath}. Expected format: V<MajorVersion>_<MinorVersion>__<description>.sql");
            }

            string[] versionParts = parts[0].Split(new string[] { "_" }, StringSplitOptions.None);
            if (versionParts.Length != 2 || !int.TryParse(versionParts[0].Substring(1), out int major) || !int.TryParse(versionParts[1], out int minor))
            {
                throw new ApplicationException($"Invalid version format in script file name: {filePath}. Expected format: V<MajorVersion>_<MinorVersion>__<description>.sql");
            }

            version = major * 100 + minor;            
        }
    }
}
