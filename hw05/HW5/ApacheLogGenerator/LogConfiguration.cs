using System.Collections.Generic;

namespace HW5.ApacheLogGenerator
{
    public class LogConfiguration
    {
        public string OutputFilePath { get; }
        public string ClientIdentity { get; } = "-";
        public IList<string> LogLineFormat { get; }
        public IList<string> IpAddressPool { get; }
        public IList<string> UserIdPool { get; }

        public LogConfiguration(IList<string> logLineFormat, IList<string> ipAddressPool,
            IList<string> userIdPool, string outputFilePath)
        {
            LogLineFormat = logLineFormat;
            IpAddressPool = ipAddressPool;
            UserIdPool = userIdPool;
            OutputFilePath = outputFilePath;
        }
    }
}
