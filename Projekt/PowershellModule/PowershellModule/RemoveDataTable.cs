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
    /// <para type="synopsis">Delete table from database.</para>
    /// <para type="description">This cmdlet delete all rows from provided table in database.</para>
    /// <para type="description">Rows are compared by DataTable primary key. After execution table always remain in database although it could be empty. (So it is not equivalent to drop)</para>
    /// </summary>
    /// <example>
    ///   <para>We have part of table which we want to delete stored in variable, then pass it through pipeline to cmdlet Remove-DataTable which will process it and remove desired rows.</para>
    ///   <code>$changedTable | Remove-DataTable $conn</code>
    /// </example>
    [Alias("sqldelete")]
    [Cmdlet(VerbsCommon.Remove, "DataTable")]
    public class RemoveDataTable : AsyncCmdlet
    {
        /// <summary>
        /// <para type="description">Connection to SQL database.</para>
        /// </summary>
        [Parameter(
            Mandatory = true,
            Position = 0,
            HelpMessage = "Connection to SQL database."
        )]
        public SqlConnection Connection { get; set; }

        /// <summary>
        /// <para type="description">Table rows which will be deleted from database.</para>
        /// </summary>
        [Parameter(
            Mandatory = true,
            ValueFromPipeline = true,
            Position = 1,
            HelpMessage = "Table rows which will be deleted from database."
        )]
        public DataTable Table { get; set; }

        /// <summary>
        /// <para type="description">Async begin processing.</para>
        /// </summary>
        /// <returns>Task with async opened Connection (if not yet opened).</returns>
        async protected override Task BeginProcessingAsync()
        {
            if (Connection.State != ConnectionState.Open)
            {
                await Connection.OpenAsync();
                WriteVerbose("Remove-Rows: Connection successfully opened");
            }
        }

        /// <summary>
        /// <para type="description">Async process record.</para>
        /// </summary>
        /// <returns>Task that will be complete when all rows are removed.</returns>
        protected override Task ProcessRecordAsync()
        {
            Validators.ValidateTableName(Table, "Table");
            Validators.ValidateTablePrimaryKey(Table, "Table");
            
            return Task.WhenAll(Table.AsEnumerable().AsParallel().Select(ProcessRow));
        }

        /// <summary>
        /// <para type="description">Async end processing. Close connection</para>
        /// </summary>
        /// <returns>Completed task.</returns>
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
