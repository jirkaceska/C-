using HW5.ApacheLogGenerator;
using HW5.Deserializers;
using HW5.Enums;
using HW5.LogManipulators;
using HW5.Utils;
using System;
using System.Text.RegularExpressions;

namespace HW5
{
    class Program
    {
        static void Main(string[] args)
        {
            JsonConfigurationDeserializer jsonConfigurationDeserializer = new JsonConfigurationDeserializer();
            XmlConfigurationDeserializer xmlConfigurationDeserializer = new XmlConfigurationDeserializer();

            RandomItemAccess randomItemAccess = new RandomItemAccess();
            LogLineDirector logLineDirector = new LogLineDirector();

            LogGenerator jsonConfigurationLogGenerator = new LogGenerator(jsonConfigurationDeserializer.Deserialize(@"..\..\InputData\LogConfiguration_.json"), randomItemAccess, logLineDirector);
            jsonConfigurationLogGenerator.GenerateLog();

            LogGenerator xmlConfigurationLogGenerator = new LogGenerator(xmlConfigurationDeserializer.Deserialize(@"..\..\InputData\LogConfiguration_.xml"), randomItemAccess, logLineDirector);
            xmlConfigurationLogGenerator.GenerateLog();

            /*string fileName = @"..\..\OutputData\json_correctlog1.txt";
            Mutator mutator = new Mutator();
            mutator.HideDateByRandomDate(fileName);
            mutator.HideIpAddressByLocalhost(fileName);

            Analyzer analyzer = new Analyzer();
            analyzer.GetNumberOfClassStatusCodes(fileName, HttpStatusClass.ClientError);

            Validator validator = new Validator();
            validator.ValidateRandomLogs(fileName, "%h %l %u %t %r %s %b");*/
        }
    }
}
