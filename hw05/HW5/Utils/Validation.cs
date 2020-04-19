using HW5.ApacheLogGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HW5.Utils
{
    public static class Validation
    {
        public static bool IsLogConfigurationValid(LogConfiguration logConfiguration)
        {
            return logConfiguration.IpAddressPool
                .All(address => IPAddress.TryParse(address, out _));
        }

        public static string GetPattern(string token)
        {
            switch (token)
            {
                case "%h":
                    return @"((25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9]?[0-9])\.){3}(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9]?[0-9])";
                case "%l":
                    return @"-";
                case "%u":
                    return @"\w+";
                case "%t":
                    return @"((0[1-9])|([12][0 - 9])|(3[01]))\/((0[1-9])|1[0-2])\/\d\d\d\d:(2[0-3]|[0-1]\d):[0-5]\d:[0-5]\d \+0100";
                case "%r":
                    return @"""((Get)|(Head)|(Post)|(Put)|(Delete)|(Trace)|(Options)|(Connect)|(Patch)) \/([\/\w .])+""";
                case "%s":
                    return @"((10[0-3])|([2-5]0[0-8])|226|409|(41[0-8])|(42[1-689])|431|451|510|511)";
                case "%b":
                    return @"[1-9]\d{3,}";
                default:
                    return "";
            }
        }
}
}
