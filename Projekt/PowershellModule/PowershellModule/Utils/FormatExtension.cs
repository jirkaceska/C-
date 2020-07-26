using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Utils
{
    static class FormatExtension
    {
        public static string FormatKeyValuePair(KeyValuePair<string, object> keyValuePair)
        {
            if (keyValuePair.Value == null)
            {
                return $"{keyValuePair.Key} = 'NULL'";
            }
            return $"{keyValuePair.Key} = '{keyValuePair.Value.ToString().Replace("'", "''")}'";
        }

        public static string TupleConcat(this IEnumerable<object> columns)
        {
            return String.Join(", ", columns);
        }

        public static string WhereConcat(this IEnumerable<object> columns)
        {
            return String.Join(" AND ", columns);
        }

        public static string ConcatWithQuotes(this IEnumerable<object> values)
        {
            return values.Select(value => (value.ToString()).Wrap('\'', '\'')).ToArray().TupleConcat();
        }

        public static string Wrap(this string str, char fst='(', char snd=')')
        {
            var builder = new StringBuilder();
            builder.Append(fst);
            builder.Append(str);
            builder.Append(snd);
            return builder.ToString();
        }

        public static string EmptyOrWrapped(this IEnumerable<object> columns)
        {
            if (columns == null || columns.Count() == 0) {
                return "";
            }
            return columns.ConcatAndWrap();
        }

        public static string ConcatAndWrap(this IEnumerable<object> columns)
        {
            return columns.TupleConcat().Wrap();
        }
    }
}
