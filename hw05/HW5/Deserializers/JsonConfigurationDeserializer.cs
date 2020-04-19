using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HW5.ApacheLogGenerator;
using HW5.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HW5.Deserializers
{
    public class JsonConfigurationDeserializer : IConfigurationDeserializer
    {
        public IEnumerable<LogConfiguration> Deserialize(string inputFilePath)
        {
            try
            {
                return JArray.Parse(File.ReadAllText(inputFilePath))
                    .Select(logConf => new LogConfiguration(
                        ((string)logConf["Format"]).Split(' '),
                        logConf["IPAddresses"].ToObject<IList<string>>(),
                        logConf["UserIds"].ToObject<IList<string>>(),
                        (string)logConf["OutputFilepath"]
                    ))
                    .Where(Validation.IsLogConfigurationValid);
            } 
            catch(Exception e) when (e is FileNotFoundException || e is ArgumentException)
            {
                Console.Error.WriteLine("File path is invalid!");
                
            }
            catch (ArgumentNullException _)
            {
                Console.Error.WriteLine("File path cannot be null!");
            }
            catch (IOException _)
            {
                Console.Error.WriteLine("File cannot be null!");
            } 
            catch(JsonSerializationException e)
            {
                Console.Error.WriteLine(e);
            }
            return new List<LogConfiguration>();
        }
    }
}
