using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HW5.ApacheLogGenerator
{
    public class LogLineDirector
    {
   

        public void ConstructLogLine(ILogLineBuilder builder, IList<string> logLineFormat)
        {
            foreach (var element in logLineFormat)
            {
                BuildElement(builder, element);
            }
        }

        private void BuildElement(ILogLineBuilder builder, string element)
        {
            switch (element)
            {
                case "%h":
                    builder.GenerateIpAddress();
                    break;
                case "%l":
                    builder.GenerateClientIdentity();
                    break;
                case "%u":
                    builder.GenerateUserId();
                    break;
                case "%t":
                    builder.GenerateDateTime();
                    break;
                case "%r":
                    builder.GenerateHttpRequest();
                    break;
                case "%s":
                    builder.GenerateStatusCode();
                    break;
                case "%b":
                    builder.GenerateObjectSize();
                    break;
            }
        }
    }
}
