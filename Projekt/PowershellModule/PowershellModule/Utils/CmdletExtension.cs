using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace Database.Utils
{
    public static class CmdletExtension
    {
        public static void WriteError(this Cmdlet cmdlet, Exception e, ErrorCategory category)
        {
            var errorRecord = new ErrorRecord(e, e.Message, category, cmdlet);
            cmdlet.WriteError(errorRecord);
        }

        public static IEnumerable<PSObject> Execute(this ScriptBlock script, DataRow input)
        {
            IEnumerable<PSObject> result = script.InvokeWithContext(null, new List<PSVariable>() { input.ToPSVariable("_") }, null);
            if (result == null)
            {
                throw new ArgumentNullException($"{input.ToKeyValuePairList().ConcatAndWrap()} does not have property stated in script '{script.ToString()}'");
            }
            return result;
        }

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
