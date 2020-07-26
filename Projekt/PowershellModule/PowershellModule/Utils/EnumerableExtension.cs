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
    /// <summary>
    /// <para type="description">Extension for enumerable or like enumerable (DataRow, PSObject).</para>
    /// </summary>
    static class EnumerableExtension
    {
        /// <summary>
        /// <para type="description">Select list of key value pairs from data row based on table primary key.</para>
        /// <para type="description">Row will be processed to list of (columnName, correspondingValue) pair.</para>
        /// </summary>
        /// <param name="row">Row to process.</param>
        /// <param name="include">If true, only primary key columns will be select, otherwise all other columns.</param>
        /// <returns>List of pairs of column name and corresponding value.</returns>
        public static IEnumerable<KeyValuePair<string, object>> SelectKeyValuePairByPrimaryKey(this DataRow row, bool include)
        {
            var primaryKey = new HashSet<string>(row.Table.PrimaryKey.Select(key => key.ColumnName));
            return row.SelectKeyValuePair().Where(column => primaryKey.Contains(column.Key) ^ !include);
        }

        /// <summary>
        /// <para type="description">Select list of key value pairs from data row.</para>
        /// <para type="description">Row will be processed to list of (columnName, correspondingValue) pair.</para>
        /// </summary>
        /// <param name="row">Row to process.</param>
        /// <returns>List of pairs of column name and corresponding value.</returns>
        public static IEnumerable<KeyValuePair<string, object>> SelectKeyValuePair(this DataRow row)
        {
            return row.ItemArray.Select((value, i) => new KeyValuePair<string, object>(row.Table.Columns[i].ColumnName, value));
        }

        /// <summary>
        /// <para type="description">Select list of key value pairs from data row.</para>
        /// <para type="description">Row will be processed to list of (columnName, correspondingValue) pair.</para>
        /// </summary>
        /// <typeparam name="TResult">Type of value of KeyValuePair</typeparam>
        /// <param name="row">Row to process.</param>
        /// <param name="table">Table to check columns against.</param>
        /// <param name="selector">Function to extract key value pair from row properties.</param>
        /// <returns>List of pairs of column name and corresponding value.</returns>
        public static IEnumerable<KeyValuePair<string, TResult>> SelectKeyValuePair<TResult>(this PSObject row, DataTable table, Func<PSPropertyInfo, KeyValuePair<string, TResult>> selector)
        {
            return table.Columns.Cast<DataColumn>().Select(column => row.Properties[column.ColumnName]).Select(selector);
        }

        /// <summary>
        /// <para type="description">Transform row in list of strings.</para>
        /// </summary>
        /// <param name="row">Row to process.</param>
        /// <returns>List of strings in form columnName='correspondingValue'. Apostrophes in value are escaped.</returns>
        public static IEnumerable<string> ToKeyValuePairList(this DataRow row)
        {
            return row.SelectKeyValuePair().Select(FormatExtension.FormatKeyValuePair);
        }

        /// <summary>
        /// <para type="description">Substitution of classic Select, convert to Collection instead of IEnumerable.</para>
        /// </summary>
        /// <typeparam name="TInput">Type of items in collection.</typeparam>
        /// <typeparam name="TResult">Types of items in output Collection.</typeparam>
        /// <param name="collection">Collection to select.</param>
        /// <param name="selector">Selector function.</param>
        /// <returns>Selected collection.</returns>
        public static Collection<TResult> SelectT<TInput, TResult>(this IEnumerable<TInput> collection, Func<TInput, TResult> selector)
        {
            var output = new Collection<TResult>();
            foreach (var item in collection)
            {
                output.Add(selector(item));
            }
            return output;
        }

        /// <summary>
        /// <para type="description">Prepare DataRow to input to PSScript.</para>
        /// </summary>
        /// <param name="row">Row to process.</param>
        /// <param name="variableName">Name of variable in script.</param>
        /// <returns>New PSVariable.</returns>
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
