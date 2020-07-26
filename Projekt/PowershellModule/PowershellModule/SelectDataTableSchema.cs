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
    [Cmdlet(VerbsCommon.Select, "DataTableSchema")]
    public class SelectDataTableSchema : SelectDataTable
    {
        public override string GetCommandText() => $"SELECT TOP 0 {Columns.TupleConcat()} FROM {Table};";
    }
}
