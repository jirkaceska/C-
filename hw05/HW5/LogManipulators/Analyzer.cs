using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HW5.Enums;

namespace HW5.LogManipulators
{
    public class Analyzer
    {
        public uint GetNumberOfClassStatusCodes(string filepath, HttpStatusClass statusClass)
        {
            var pattern = @"\b" + (int)statusClass + @"\d\d\b";
            uint counter = 0;
            string line;
            using (var reader = new StreamReader(filepath))
            {
                while (null != (line = reader.ReadLine()))
                {
                    if (Regex.Match(line, pattern).Success)
                    {
                        counter += 1;
                    }
                }
            }
            return counter;
        }
    }
}
