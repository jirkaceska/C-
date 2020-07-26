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
    [Cmdlet(VerbsCommon.Select, "DataTable")]
    public class SelectDataTable : Cmdlet
    {
        [Parameter(
            Mandatory = true,
            Position = 0,
            HelpMessage = "Connection to SQL database."
        )]
        public SqlConnection Connection { get; set; }

        [Parameter(
            Mandatory = true,
            Position = 1,
            HelpMessage = "Table to investigate."
        )]
        public string Table { get; set; }

        [Parameter(
            Position = 2,
            HelpMessage = "Columns to select."
        )]
        public string[] Columns { get; set; } = { "*" };

        public DataTable Result { get; protected set; } = new DataTable();

        protected override void BeginProcessing()
        {
            if (Connection.State != ConnectionState.Open)
            {
                Connection.Open();
                WriteVerbose("Select-Rows: Connection successfully opened");
            }
        }

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

        public virtual string GetCommandText() => $"SELECT {Columns.TupleConcat()} FROM {Table};";
    }
}
