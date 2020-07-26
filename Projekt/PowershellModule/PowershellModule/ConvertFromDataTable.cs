using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.Data.SqlClient;
using System.Data;
using Database.Utils;
using TTRider.PowerShellAsync;

namespace Database
{
    [Cmdlet(VerbsData.ConvertFrom, "DataTable")]
    public class ConvertFromDataTable : Cmdlet
    {
        [Parameter(
            Mandatory = true,
            ValueFromPipelineByPropertyName = true,
            ValueFromPipeline = true,
            Position = 0,
            HelpMessage = "Table to split."
        )]
        public DataTable Table { get; set; }

        protected override void ProcessRecord()
        {
            foreach (DataRow row in Table.Rows)
            {
                var output = new PSObject();
                for (int i = 0; i < row.ItemArray.Length; i++)
                {
                    output.Properties.Add(new PSNoteProperty(Table.Columns[i].ColumnName, row.ItemArray[i]));
                }
                WriteObject(output);
            }
        }
    }
}
