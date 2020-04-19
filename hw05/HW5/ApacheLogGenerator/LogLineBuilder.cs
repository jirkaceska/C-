using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HW5.Enums;

namespace HW5.ApacheLogGenerator
{

    public class LogLineBuilder : ILogLineBuilder
    {
        private const int LowestObjectSize = 1000;
        private const int HighestObjectSize = 200000;

        private readonly StringBuilder _logLine = new StringBuilder();

        private readonly RandomItemAccess _randomItemAccess;
        private readonly LogConfiguration _configuration;
        readonly Random _random = new Random();

        public LogLineBuilder(LogConfiguration configuration, RandomItemAccess randomItemAccess)
        {
            _configuration = configuration;
            _randomItemAccess = randomItemAccess;
        }

        public void GenerateIpAddress()
        {
            AppendElement(_randomItemAccess.RetrieveRandomItem(_configuration.IpAddressPool));
        }

        public void GenerateClientIdentity()
        {
            AppendElement(_configuration.ClientIdentity);
        }

        public void GenerateUserId()
        {
            AppendElement(_randomItemAccess.RetrieveRandomItem(_configuration.UserIdPool));
        }

        public void GenerateDateTime()
        {
            AppendElement(DateTime.Now.ToString("dd/MM/yyyy:HH:mm:ss", CultureInfo.InvariantCulture) + " +0100");
        }

        public void GenerateHttpRequest()
        {
            AppendElement(CreateHttpRequest());
        }

        public void GenerateStatusCode()
        {
            AppendElement(((int)RetrieveRandomEnumValue<HttpStatusCode>()).ToString());
        }

        public void GenerateObjectSize()
        {
            AppendElement(_random.Next(LowestObjectSize, HighestObjectSize).ToString());
        }

        public string GetLogLine()
        {
            RemoveLastCharFromBuilder();
            return _logLine.ToString();
        }

        private void AppendElement(string element)
        {
            _logLine.Append(element);
            _logLine.Append(" ");
        }

        private string CreateHttpRequest()
        {
            string method = RetrieveRandomEnumValue<HttpMethod>().ToString();
            string protocol = "HTTP/1.0";
            string requestRoute = CreateRandomRequestRoute();
            string request = string.Join(" ", method, requestRoute, protocol);
            return request.Insert(0, "\"").Insert(request.Length + 1, "\"");
        }

        private string CreateRandomRequestRoute()
        {
            StringBuilder path = new StringBuilder();

            for (int i = 0; i < _random.Next(0, 5); i++)
            {
                path.Append("/").Append(Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));
            }

            if (_random.Next(0, 1) == 0)
            {
                path.Append("/").Append(Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));
            }
            else
            {
                path.Append("/").Append(Path.GetRandomFileName());
            }

            return path.ToString();
        }

        private T RetrieveRandomEnumValue<T>() where T : struct, IConvertible
        {
            Array values = Enum.GetValues(typeof(T));
            return (T)values.GetValue(_random.Next(values.Length));
        }

        private void RemoveLastCharFromBuilder()
        {
            _logLine.Remove(_logLine.Length - 1, 1);
        }
    }
}
