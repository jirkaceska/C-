using HW5.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HW5.LogManipulators
{
    public class Validator
    {
        public void ValidateRandomLogs(string filepath, string configuration)
        {
            var pattern = configuration.Split(' ')
                .Select(Validation.GetPattern)
                .Aggregate((result, token) => result + " " + token);

            FileUtils.ReplaceInFile(
                    filepath,
                    (line, writer) =>
                    {
                        if (Regex.Match(line, pattern).Success)
                        {
                            writer.WriteLine(line);
                        }
                    }
            );
        }
    }
}
