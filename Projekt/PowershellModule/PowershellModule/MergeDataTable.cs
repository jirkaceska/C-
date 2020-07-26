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
    /// <para type="synopsis">Upsert table in database.</para>
    /// <para type="description">This cmdlet upsert all rows from provided table in database.</para>
    /// <para type="description">Rows are compared by DataTable primary key. If match is found then row is updated, otherwise is inserted.</para>
    /// </summary>
    /// <example>
    ///   <para>We have table which was changed stored in variable, then pass it through pipeline to cmdlet Merge-DataTable which will process it and perform upsert.</para>
    ///   <code>$changedTable | Merge-DataTable $conn</code>
    /// </example>
    [Alias("sqlupsert")]
    [Cmdlet(VerbsData.Merge, "DataTable")]
    public class MergeDataTable : AsyncCmdlet
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
        /// <para type="description">Table to upsert to database.</para>
        /// </summary>
        [Parameter(
            Mandatory = true,
            ValueFromPipeline = true,
            Position = 1,
            HelpMessage = "Table to upsert to database."
        )]
        public DataTable Table { get; set; }


        /// <summary>
        /// <para type="description">Table heading formatted as tuple.</para>
        /// </summary>
        public string ColumnNamesTuple { get; private set; }

        /// <summary>
        /// <para type="description">Async begin processing.</para>
        /// </summary>
        /// <returns>Task with async opened Connection (if not yet opened).</returns>
        async protected override Task BeginProcessingAsync()
        {
            if (Connection.State != ConnectionState.Open)
            {
                await Connection.OpenAsync();
                WriteVerbose("Merge-Rows: Connection successfully opened");
            }
        }

        /// <summary>
        /// <para type="description">Async process record.</para>
        /// </summary>
        /// <returns>Task that will be complete when all rows are upserted.</returns>
        async protected override Task ProcessRecordAsync()
        {
            Validators.ValidateTableName(Table, "Table");
            Validators.ValidateTablePrimaryKey(Table, "Table");
            
            ColumnNamesTuple = Table.Columns.Cast<DataColumn>().ConcatAndWrap();

            await Task.WhenAll(Table.AsEnumerable().AsParallel().Select(ProcessRow));
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
