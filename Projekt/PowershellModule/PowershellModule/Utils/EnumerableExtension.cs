using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace Database.Utils
{
    public static class EnumerableExtension
    {
        public static IEnumerable<KeyValuePair<string, object>> SelectKeyValuePairByPrimaryKey(this DataRow row, bool include)
        {
            var primaryKey = new HashSet<string>(row.Table.PrimaryKey.Select(key => key.ColumnName));
            return row.SelectKeyValuePair().Where(column => primaryKey.Contains(column.Key) ^ !include);
        }

        public static IEnumerable<KeyValuePair<string, object>> SelectKeyValuePair(this DataRow row)
        {
            return row.ItemArray.Select((value, i) => new KeyValuePair<string, object>(row.Table.Columns[i].ColumnName, value));
        }

        public static IEnumerable<KeyValuePair<string, TResult>> SelectKeyValuePair<TResult>(this PSObject row, Func<PSPropertyInfo, KeyValuePair<string, TResult>> selector)
        {
            return row.Properties.Select(selector);
        }

        public static IEnumerable<string> ToKeyValuePairList(this DataRow row)
        {
            return row.SelectKeyValuePair().Select(FormatExtension.FormatKeyValuePair);
        }

        public static Collection<TResult> SelectT<TInput, TResult>(this IEnumerable<TInput> collection, Func<TInput, TResult> selector)
        {
            var output = new Collection<TResult>();
            foreach (var item in collection)
            {
                output.Add(selector(item));
            }
            return output;
        }

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> items)
        {
            return items.ToDictionary(item => item.Key, item => item.Value);
        }

        public static PSVariable ToPSVariable(this DataRow row, string variableName)
        {
            var variableValue = new ExpandoObject() as IDictionary<string, Object>;
            foreach (var tuple in row.SelectKeyValuePair())
            {
                variableValue.Add(tuple.Key, tuple.Value.ToString());
            }
            return new PSVariable(variableName, variableValue);
        }
    }
}
