using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.Data.SqlClient;
using System.Data;
using Database.Utils;

namespace Database
{
    /// <summary>
    /// <para type="synopsis">Select table from database.</para>
    /// <para type="description">This cmdlet select datatable from database by provided SqlConnection (obtained by New-Connection cmdlet).</para>
    /// </summary>
    /// <example>
    ///   <para>Select table MyTable from DB specified by connection $conn.</para>
    ///   <code>Select-DataTable $conn MyTable</code>
    /// </example>
    [Alias("sqlselect")]
    [Cmdlet(VerbsCommon.Select, "DataTable")]
    [OutputType(typeof(DataTable))]
    public class SelectDataTable : Cmdlet
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
        /// <para type="description">Table to investigate.</para>
        /// </summary>
        [Parameter(
            Mandatory = true,
            Position = 1,
            HelpMessage = "Table to investigate."
        )]
        public string Table { get; set; }

        /// <summary>
        /// <para type="description">Columns to select.</para>
        /// </summary>
        [Parameter(
            Position = 2,
            HelpMessage = "Columns to select."
        )]
        public string[] Columns { get; set; } = { "*" };

        /// <summary>
        /// <para type="description">Output of cmdlet - selected DataTable.</para>
        /// </summary>
        public DataTable Result { get; protected set; } = new DataTable();

        /// <summary>
        /// <para type="description">Begin processing.</para>
        /// </summary>
        protected override void BeginProcessing()
        {
            if (Connection.State != ConnectionState.Open)
            {
                Connection.Open();
                WriteVerbose("Select-Rows: Connection successfully opened");
            }
        }

        /// <summary>
        /// <para type="description">ProcessRecord</para>
        /// </summary>
        protected override void ProcessRecord()
        {
            var command = Connection.CreateCommand();
            command.CommandText = GetCommandText();
            WriteVerbose(command.CommandText);

            try
            {
                using (var reader = command.ExecuteReader(CommandBehavior.KeyInfo))
                {
                    Result = new DataTable
                    {
                        TableName = Table
                    };
                    Result.Load(reader);
                    WriteVerbose("Select-Rows: Data stored in result");
                    WriteObject(Result, false);
                }
            }
            catch (SqlException e)
            {
                WriteWarning(e.Message);
                throw;
            }
        }

        /// <summary>
        /// <para type="description">Get Sql command text to select table.</para>
        /// </summary>
        /// <returns>Command Text.</returns>
        public virtual string GetCommandText() => $"SELECT {Columns.TupleConcat()} FROM {Table};";
    }
}
