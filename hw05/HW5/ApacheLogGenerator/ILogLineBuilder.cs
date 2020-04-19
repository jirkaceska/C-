using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW5.ApacheLogGenerator
{
    public interface ILogLineBuilder
    {
        void GenerateIpAddress();

        void GenerateClientIdentity();

        void GenerateUserId();

        void GenerateDateTime();

        void GenerateHttpRequest();

        void GenerateStatusCode();

        void GenerateObjectSize();

        string GetLogLine();
    }
}
