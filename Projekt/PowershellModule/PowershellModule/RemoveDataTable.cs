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
    [Cmdlet(VerbsCommon.Remove, "DataTable")]
    public class RemoveDataTable : AsyncCmdlet
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
            HelpMessage = "Table rows which will be deleted from database."
        )]
        public DataTable Table { get; set; }

        async protected override Task BeginProcessingAsync()
        {
            if (Connection.State != ConnectionState.Open)
            {
                await Connection.OpenAsync();
                WriteVerbose("Remove-Rows: Connection successfully opened");
            }
        }

        protected override Task ProcessRecordAsync()
        {
            if (Table.TableName == null)
            {
                throw new ArgumentException("Table does not have specified TableName property!");
            }
            return Task.WhenAll(Table.AsEnumerable().AsParallel().Select(ProcessRow));
        }

        protected override Task EndProcessingAsync()
        {
            if (Connection.State == ConnectionState.Open)
            {
                Connection.Close();
                WriteVerbose("Remove-Rows: Connection closed successfully.");
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
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    WriteVerbose($"Remove-Rows: Delete {row.ToKeyValuePairList().ConcatAndWrap()} successfull.");
                }
            }
            catch (SqlException e)
            {
                WriteWarning(e.Message);
                throw;
            }
        }

        private string GetCommandText(DataRow row)
        {
            return String.Format(
                @"SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
                BEGIN TRAN
                
                DELETE FROM {0} WHERE {1};
 
                COMMIT",
                Table.TableName,
                row.SelectKeyValuePairByPrimaryKey(true).Select(FormatExtension.FormatKeyValuePair).WhereConcat()
            );
        }
    }
}
