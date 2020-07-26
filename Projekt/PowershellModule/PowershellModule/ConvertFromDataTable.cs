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
    /// <summary>
    /// <para type="synopsis">Convert DataTable to enumeration of rows.</para>
    /// <para type="description">Needed for processing by Powershell cmdlets like select, where, foreach, Export-Csv</para>
    /// </summary>
    /// <example>
    ///   <para>Select first 5 rows of table $table.</para>
    ///   <code>ConvertFrom-DataTable $table | select -First 5</code>
    /// </example>
    /// <example>
    ///   <para>Select column Id, Name of table $table.</para>
    ///   <code>ConvertFrom-DataTable $table | select -Property Id, Name</code>
    /// </example>
    /// <example>
    ///   <para>Export table $table to CSV.</para>
    ///   <code>ConvertFrom-DataTable $table | Export-Csv -Path .\test.csv</code>
    /// </example>
    [Cmdlet(VerbsData.ConvertFrom, "DataTable")]
    [OutputType(typeof(DataRow[]))]
    public class ConvertFromDataTable : Cmdlet
    {
        /// <summary>
        /// <para type="description">Table to convert.</para>
        /// </summary>
        [Parameter(
            Mandatory = true,
            ValueFromPipelineByPropertyName = true,
            ValueFromPipeline = true,
            Position = 0,
            HelpMessage = "Table to convert."
        )]
        public DataTable Table { get; set; }

        /// <summary>
        /// <para type="description">Process record.</para>
        /// </summary>
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
