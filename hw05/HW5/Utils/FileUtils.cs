using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HW5.Utils
{
    public static class FileUtils
    {
        public static string TmpFileName(string filePath)
        {
            return new StringBuilder(Path.GetDirectoryName(filePath))
                .Append(Path.DirectorySeparatorChar)
                .Append(Path.GetFileNameWithoutExtension(filePath))
                .Append("_tmp")
                .Append(Path.GetExtension(filePath))
                .ToString();
        }

        public static void ReplaceInFile(string filePath, Action<string,StreamWriter> replaceAction)
        {
            
            string tempPath = TmpFileName(filePath);
            string line;
            using (var reader = new StreamReader(filePath))
            {
                using (var writer = new StreamWriter(tempPath))
                {
                    while (null != (line = reader.ReadLine()))
                    {
                        replaceAction(line, writer);
                    }
                }
            }
            File.Delete(filePath);
            File.Move(tempPath, filePath);
        }
    }
}
