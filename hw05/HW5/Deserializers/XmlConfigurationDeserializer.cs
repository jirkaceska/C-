using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using HW5.ApacheLogGenerator;
using HW5.Utils;

namespace HW5.Deserializers
{
    public class XmlConfigurationDeserializer : IConfigurationDeserializer
    {
        public IEnumerable<LogConfiguration> Deserialize(string inputFilePath)
        {
            try
            {
                var root = XElement.Load(inputFilePath);
                return root.Elements("Configuration")
                    .Select(logConf => new LogConfiguration(
                        logConf.Element("Format").Value.Split(' '),
                        logConf.Element("IPAddresses").Elements("IPAddress")
                            .Select(address => address.Value)
                            .ToList(),
                        logConf.Element("UserIds").Elements("UserId")
                            .Select(userId => userId.Value)
                            .ToList(),
                        logConf.Attribute("output_filepath").Value
                    ))
                    .Where(Validation.IsLogConfigurationValid);
            }
            catch (Exception e) when (e is FileNotFoundException || e is ArgumentException)
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
            catch (XmlException e)
            {
                Console.Error.WriteLine(e);
            }
            return new List<LogConfiguration>();
        }
    }
}
