using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace Database.Utils
{
    /// <summary>
    /// <para type="description">Extension for Cmdlet and ScriptBlock.</para>
    /// </summary>
    static class CmdletExtension
    {
        /// <summary>
        /// <para type="description">Print error message to command line based on exception.</para>
        /// </summary>
        /// <param name="cmdlet">Cmdlet which print message</param>
        /// <param name="e">Exception</param>
        /// <param name="category">Error category</param>
        public static void WriteError(this Cmdlet cmdlet, Exception e, ErrorCategory category)
        {
            var errorRecord = new ErrorRecord(e, e.Message, category, cmdlet);
            cmdlet.WriteError(errorRecord);
        }

        /// <summary>
        /// <para type="description">Execute script and return its result. Accepts one input.</para>
        /// </summary>
        /// <param name="script">Script to execute.</param>
        /// <param name="input">DataRow which will be input of script.</param>
        /// <returns>Result of script.</returns>
        /// <exception cref="ArgumentNullException">If result of script is null. That could mean that script is invalid or is using nonexisting property of provided DataRow.</exception>
        public static IEnumerable<PSObject> Execute(this ScriptBlock script, DataRow input)
        {
            IEnumerable<PSObject> result = script.InvokeWithContext(null, new List<PSVariable>() { input.ToPSVariable("_") }, null);
            if (result == null)
            {
                throw new ArgumentNullException($"{input.ToKeyValuePairList().ConcatAndWrap()} does not have property stated in script '{script.ToString()}'");
            }
            return result;
        }

        /// <summary>
        /// <para type="description">Execute script and return its result. Accepts two inputs.</para>
        /// </summary>
        /// <param name="script">Script to execute.</param>
        /// <param name="outer">DataRow which will be first input of script.</param>
        /// <param name="inner">DataRow which will be second input of script.</param>
        /// <returns>Result of script.</returns>
        /// <exception cref="ArgumentNullException">If result of script is null. That could mean that script is invalid or is using nonexisting property of provided DataRows.</exception>
        public static IEnumerable<object> Execute(this ScriptBlock script, DataRow outer, DataRow inner)
        {
            IEnumerable<PSObject> result = script.InvokeWithContext(null, new List<PSVariable>() { outer.ToPSVariable("outer"), inner.ToPSVariable("inner") }, null);
            if (result == null)
            {
                throw new ArgumentNullException($"{outer.ToKeyValuePairList().Concat(inner.ToKeyValuePairList()).ConcatAndWrap()} " +
                    $"does not have property stated in script '{script.ToString()}'");
            }
            return result.Select(obj => obj.BaseObject);
        }

        /// <summary>
        /// <para type="description">Parse ScriptBlock command to pairs of tables and used columns.</para>
        /// </summary>
        /// <param name="script">Script which will be parsed.</param>
        /// <returns>List of pairs (tableName, itsColumnName).</returns>
        public static IEnumerable<KeyValuePair<string, string>> GetSelectedColumns(this ScriptBlock script)
        {
            var variables = script.ToString().Trim('@').Trim('(').Trim(')').Split(',').Select(variable => variable.Trim());
            return variables.Select(VariableToTableAndColum);
        }

        private static KeyValuePair<string, string> VariableToTableAndColum(string variable)
        {
            var tokens = variable.Split('.');
            return new KeyValuePair<string, string>(
                tokens[0].Trim('$'),
                tokens[1]
            );
        }
    }
}
