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
    [Cmdlet(VerbsData.Merge, "DataTable")]
    public class MergeDataTable : AsyncCmdlet
    {
        [Parameter(
            Mandatory = true,
            Position = 0,
            HelpMessage = "Connection to SQL database."
        )]
        public SqlConnection Connection { get; set; }

        [Parameter(
            Mandatory = true,
            ValueFromPipeline = true,
            Position = 1,
            HelpMessage = "Table to upsert to database"
        )]
        public DataTable Table { get; set; }
        

        public string ColumnNamesTuple { get; private set; }

        async protected override Task BeginProcessingAsync()
        {
            if (Connection.State != ConnectionState.Open)
            {
                await Connection.OpenAsync();
                WriteVerbose("Merge-Rows: Connection successfully opened");
            }
        }

        async protected override Task ProcessRecordAsync()
        {
            if (Table.PrimaryKey.Length == 0)
            {
                throw new ArgumentException("Primary key columns indices must not be empty!");
            }
            if (Table.TableName == null)
            {
                throw new ArgumentException("Table does not have specified TableName property!");
            }
            ColumnNamesTuple = Table.Columns.Cast<DataColumn>().ConcatAndWrap();

            await Task.WhenAll(Table.AsEnumerable().AsParallel().Select(ProcessRow));
        }

        protected override Task EndProcessingAsync()
        {
            if (Connection.State == ConnectionState.Open)
            {
                Connection.Close();
                WriteVerbose("Merge-Rows: Connection closed successfully.");
            }
            return Task.CompletedTask;
        }

        private async Task ProcessRow(DataRow row)
        {
            var commandText = GetCommandText(row);

            var command = Connection.CreateCommand();
            command.CommandText = commandText;

            WriteDebug(commandText);

            try
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    WriteVerbose($"Merge-Rows: Upsert {row.ToKeyValuePairList().ConcatAndWrap()} successfull.");
                }
            }
            catch (SqlException e)
            {
                this.WriteError(e, ErrorCategory.WriteError);
                throw;
            }
        }

        private string GetCommandText(DataRow row)
        {
            return String.Format(
                @"SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
                BEGIN TRAN
 
                IF EXISTS ( SELECT * FROM {0} WITH (UPDLOCK) WHERE {1} )
 
                    UPDATE {0}
                        SET {2}
                        WHERE {1};
 
                ELSE 
 
                    INSERT INTO {0} {3}
                        VALUES {4};
 
                COMMIT",
                Table.TableName,
                row.SelectKeyValuePairByPrimaryKey(true).Select(FormatExtension.FormatKeyValuePair).WhereConcat(),
                row.SelectKeyValuePairByPrimaryKey(false).Select(FormatExtension.FormatKeyValuePair).TupleConcat(),
                ColumnNamesTuple,
                row.ItemArray.ConcatWithQuotes().Wrap()
            );
        }
    }
}
