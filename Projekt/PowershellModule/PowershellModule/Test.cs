using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Data.SqlClient;
using System.Data;
using Database.Utils;
using TTRider.PowerShellAsync;
using System.Collections.ObjectModel;
using System.Dynamic;

namespace Database
{
    [Cmdlet(VerbsCommon.Get, "Greeting")]
    public class Test : PSCmdlet
    {
        [Parameter(
            Mandatory = true,
            Position = 0,
            HelpMessage = "Connection to SQL database."
        )]
        public ScriptBlock Block { get; set; }

        public string Name { get; set; }
        protected override void ProcessRecord()
        {
            var Person1 = new ExpandoObject() as IDictionary<string, Object>;
            Person1.Add("Name", "Pepa");
            Person1.Add("Surname", "Novak");

            var Person2 = new ExpandoObject() as IDictionary<string, Object>;
            Person2.Add("Name", "Lojza");
            Person2.Add("Surname", "Novak");

            PSVariable fst = new PSVariable("fst", Person1);
            PSVariable snd = new PSVariable("snd", Person2);
            IEnumerable<object> scriptBlockResult3 = Block.InvokeWithContext(null, new List<PSVariable>() { fst, snd }, null );
            WriteVerbose("Invoke with context: ");
            if (scriptBlockResult3.Count() == 0)
            {
                WriteWarning("Empty result!");
            }
            WriteObject($"Hello to these: {String.Join(", ", scriptBlockResult3)}");
        }
    }
}
