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
using System.Collections.ObjectModel;

namespace Database
{
    /// <summary>
    /// <para type="synopsis">Join two DataTables.</para>
    /// <para type="description">Exectuion is based on LINQ join so it follow its syntax.</para>
    /// </summary>
    /// <example>
    ///   <para>Join two DataTables of orders and customers based on primary key and foreign key. Order id, name of customer who ordered it and order date are selected.</para>
    ///   <code>$orders | Join-DataTable $customers {$_.CustomerId} {$_.Id} {@($outer.Id, $inner.CustomerName, $outer.OrdersDate)}</code>
    /// </example>
    [Cmdlet(VerbsCommon.Join, "DataTable")]
    [OutputType(typeof(DataTable))]
    public class JoinDataTable : Cmdlet
    {
        /// <summary>
        /// <para type="description">Inner (right) table to join.</para>
        /// </summary>
        [Parameter(
            Mandatory = true,
            Position = 0,
            HelpMessage = "Inner (right) table to join."
        )]
        public DataTable InnerTable { get; set; }

        /// <summary>
        /// <para type="description">Specify outer key selector using PSScript. $_ variable stand for actual row in OuterTable.</para>
        /// </summary>
        [Parameter(
            Mandatory = true,
            Position = 1,
            HelpMessage = "Specify outer key selector using PSScript. $_ variable stand for actual row in OuterTable."
        )]
        public ScriptBlock OuterKeySelector { get; set; }

        /// <summary>
        /// <para type="description">Specify outer key selector using PSScript. $_ variable stand for actual row in InnerTable.</para>
        /// </summary>
        [Parameter(
            Mandatory = true,
            Position = 2,
            HelpMessage = "Specify outer key selector using PSScript. $_ variable stand for actual row in InnerTable."
        )]
        public ScriptBlock InnerKeySelector { get; set; }

        /// <summary>
        /// <para type="description">Specify columns which will be selected using PSScript. $outer variable stand for column in OuterTable, similarly $inner is InnerTable. Use only array to in this script. E. g. @($outer.Id, $inner.Name)</para>
        /// </summary>
        [Parameter(
            Mandatory = true,
            Position = 3,
            HelpMessage = "Specify columns which will be selected using PSScript. $outer variable stand for column in OuterTable, similarly $inner is InnerTable. " +
            "Use only array to in this script. E. g. @($outer.Id, $inner.Name)"
        )]
        public ScriptBlock ResultSelector { get; set; }

        /// <summary>
        /// <para type="description">Name of joined table. If not provided, JoinedTableName will be composed from names of tables concatenated by underscore.</para>
        /// </summary>
        [Parameter(
            Position = 4,
            HelpMessage = "Name of joined table. If not provided, JoinedTableName will be composed from names of tables concatenated by underscore."
        )]
        public string JoinedTableName { get; set; }

        /// <summary>
        /// <para type="description">Outer (left) table to join.</para>
        /// </summary>
        [Parameter(
            Mandatory = true,
            ValueFromPipeline = true,
            Position = 5,
            HelpMessage = "Outer (left) table to join."
        )]
        public DataTable OuterTable { get; set; }

        /// <summary>
        /// <para type="description">Output of cmdlet - joined DataTable.</para>
        /// </summary>
        public DataTable Result { get; private set; }

        /// <summary>
        /// <para type="description">Process record.</para>
        /// </summary>
        protected override void ProcessRecord()
        {
            Validators.ValidateTableName(OuterTable, "Outer table");
            Validators.ValidateTableName(InnerTable, "Inner table");
            
            if (JoinedTableName == null)
            {
                JoinedTableName = $"{OuterTable.TableName}_{InnerTable.TableName}";
            }
            Result = new DataTable
            {
                TableName = JoinedTableName
            };
            InsertColumns();

            var joinResult = OuterTable.AsEnumerable().Join(
                InnerTable.AsEnumerable(),
                OuterKeySelector.Execute,
                InnerKeySelector.Execute,
                ResultSelector.Execute,
                new PSScriptResultComparer()
            );

            foreach (var rowItemArray in joinResult)
            {
                var resultRow = Result.NewRow();
                resultRow.ItemArray = rowItemArray.ToArray();
                Result.Rows.Add(resultRow);
            }
        }

        /// <summary>
        /// <para type="description">End processing.</para>
        /// </summary>
        protected override void EndProcessing()
        {
            WriteObject(Result);
        }

        private void InsertColumns()
        {
            DataTable table;
            foreach (var tuple in ResultSelector.GetSelectedColumns())
            {
                switch (tuple.Key)
                {
                    case "outer":
                        table = OuterTable;
                        break;
                    case "inner":
                        table = InnerTable;
                        break;
                    default:
                        throw new ArgumentException("In result selector script were used different variables than outer and inner!");
                }
                var columnName = tuple.Value;
                var column = table.Columns[columnName];
                if (column == null)
                {
                    throw new ArgumentNullException($"{table.TableName} ({tuple.Key}) does not have column {tuple.Value}!");
                }
                Result.Columns.Add(column.ColumnName, column.DataType);
            }
        }
    }
}
