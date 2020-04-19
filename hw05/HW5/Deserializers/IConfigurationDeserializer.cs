using System.Collections.Generic;
using HW5.ApacheLogGenerator;

namespace HW5.Deserializers
{
    public interface IConfigurationDeserializer
    {
        IEnumerable<LogConfiguration> Deserialize(string inputFilePath);
    }
}
