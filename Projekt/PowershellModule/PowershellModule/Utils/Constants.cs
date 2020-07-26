using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Utils
{
    public static class Constants
    {
        public static readonly List<KeyValuePair<string, Type>> DBTypes = new List<KeyValuePair<string, Type>>()
        {
            new KeyValuePair<string, Type>("&bit" , typeof(Boolean) ),
            new KeyValuePair<string, Type>("tin&yint" , typeof(Byte) ),
            new KeyValuePair<string, Type>("bi&nary, FILESTREAM attribute, image, rowversion, timestamp, varbinary" , typeof(Byte[]) ),
            new KeyValuePair<string, Type>("&date, datetime, datetime2, smalldatetime" , typeof(DateTime) ),
            new KeyValuePair<string, Type>("datetime&offset" , typeof(DateTimeOffset) ),
            new KeyValuePair<string, Type>("&money, decimal, numeric, smallmoney" , typeof(Decimal) ),
            new KeyValuePair<string, Type>("&float" , typeof(Double) ),
            new KeyValuePair<string, Type>("&uniqueidentifier" , typeof(Guid) ),
            new KeyValuePair<string, Type>("smal&lint" , typeof(Int16) ),
            new KeyValuePair<string, Type>("&int" , typeof(Int32) ),
            new KeyValuePair<string, Type>("bi&gint" , typeof(Int64) ),
            new KeyValuePair<string, Type>("s&ql_variant" , typeof(Object) ),
            new KeyValuePair<string, Type>("&real" , typeof(Single) ),
            new KeyValuePair<string, Type>("&varchar, char, nchar, ntext, nvarchar, text" , typeof(String) ),
            new KeyValuePair<string, Type>("&time" , typeof(TimeSpan) ),
        };
    }
}
