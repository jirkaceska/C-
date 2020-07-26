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
    /// <para type="synopsis">Select table schema from database.</para>
    /// <para type="description">This cmdlet select datatable schema from database by provided SqlConnection (obtained by New-Connection cmdlet).</para>
    /// <para type="description">DataTable could be then used for populating from CSV file using cmdlets Import-Csv and ConvertTo-DataTable. Each value will be then validated by obtained schema.</para>
    /// </summary>
    /// <example>
    ///   <para>Select table schema of MyTable from DB specified by connection $conn.</para>
    ///   <code>Select-DataTableSchema $conn MyTable</code>
    /// </example>
    [Cmdlet(VerbsCommon.Select, "DataTableSchema")]
    [OutputType(typeof(DataTable))]
    public class SelectDataTableSchema : SelectDataTable
    {
        /// <summary>
        /// <para type="description">Get Sql command text to select table schema.</para>
        /// </summary>
        /// <returns>Command Text.</returns>
        public override string GetCommandText() => $"SELECT TOP 0 {Columns.TupleConcat()} FROM {Table};";
    }
}
