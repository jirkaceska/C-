using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW5.Tests
{
    static class TestFiles
    {
        public static bool AreFilesEqual(string expectedFilePath, string currentFilePath, out string errorMessage)
        {
            errorMessage = string.Empty;
            string expectedFileText = File.ReadAllText(expectedFilePath);
            string currentFileText = File.ReadAllText(currentFilePath);
            currentFileText = currentFileText.Replace("\r\n", "\n");
            expectedFileText = expectedFileText.Replace("\r\n", "\n");
            if (expectedFileText.Length == currentFileText.Length)
            {
                for (int i = 0; i < expectedFileText.Length; i++)
                {
                    if (expectedFileText[i] != currentFileText[i])
                    {
                        errorMessage = $"File are not same on index {i}";
                        return false;
                    }
                }
                return true;
            }
            errorMessage = $"Expected file length is {expectedFileText.Length}, actual is {currentFileText.Length}";
            return false;
        }

        public static string CreateTempFile(string originalFilePath)
        {
            string fileName = Path.GetFileName(originalFilePath);
            string tempFilePath = $@"..\..\TempFiles\{fileName}";
            DeleteFile(tempFilePath);
            File.Copy(originalFilePath, tempFilePath);
            return tempFilePath;
        }

        public static void DeleteFile(string filePath)
        {
            File.Delete(filePath);
        }
    }
}
