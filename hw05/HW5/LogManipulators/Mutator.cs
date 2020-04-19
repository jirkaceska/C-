using HW5.Utils;
using System;
using System.Text.RegularExpressions;

namespace HW5.LogManipulators
{
    public class Mutator
    {
        public void HideDateByRandomDate(string filepath)
        {
            var pattern = Validation.GetPattern("%t");

            var rnd = new Random();
            var start = new DateTime(1970, 1, 1);
            var end = new DateTime(2017, 12, 31);

            MutatorHelper(
                filepath,
                pattern,
                _ => rnd.NextDateTime(start, end).ToString("dd/MM/yyyy:HH:mm:ss") + "+0100"
            );
        }

        public void HideIpAddressByLocalhost(string filepath)
        {
            var pattern = Validation.GetPattern("%h");

            MutatorHelper(
                filepath,
                pattern,
                _ => "127.0.0.1"
            );
        }

        private static void MutatorHelper(string filePath, string pattern, Func<Match, string> mutateFunc)
        {
            var evaluator = new MatchEvaluator(mutateFunc);

            FileUtils.ReplaceInFile(
                filePath,
                (line, writer) => writer.WriteLine(Regex.Replace(line, pattern, evaluator))
            );
        }
    }
}