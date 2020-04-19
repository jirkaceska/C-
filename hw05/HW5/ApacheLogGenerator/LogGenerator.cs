using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using HttpMethod = HW5.Enums.HttpMethod;

namespace HW5.ApacheLogGenerator
{
    public class LogGenerator
    {
        private const int LowestLinesNumber = 10000;
        private const int HighestLinesNumber = 20000;

        private readonly Random _random = new Random();

        private readonly IEnumerable<LogConfiguration> _configurations;
        private readonly RandomItemAccess _randomItemAccess;
        private readonly LogLineDirector _logLineDirector;

        public LogGenerator(IEnumerable<LogConfiguration> configurations, RandomItemAccess randomItemAccess, 
            LogLineDirector logLineDirector)
        {
            _configurations = configurations;
            _randomItemAccess = randomItemAccess;
            _logLineDirector = logLineDirector;
        }

        public void GenerateLog()
        {
            foreach (var configuration in _configurations)
            {
                using (StreamWriter streamWriter = new StreamWriter(configuration.OutputFilePath))
                {
                    for (int i = 0; i < _random.Next(LowestLinesNumber, HighestLinesNumber); i++)
                    {
                        ILogLineBuilder builder = new LogLineBuilder(configuration, _randomItemAccess);
                        _logLineDirector.ConstructLogLine(builder, configuration.LogLineFormat);
                        streamWriter.WriteLine(builder.GetLogLine());
                    }
                    streamWriter.Flush();
                }
            }
        }
    }
}

