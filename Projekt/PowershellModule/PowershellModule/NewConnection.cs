using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.Data.SqlClient;
using System.Data;

namespace Database
{
    [Cmdlet(VerbsCommon.New, "Connection")]
    public class NewConnection : Cmdlet
    {
        [Parameter(
            Mandatory = true,
            Position = 0,
            HelpMessage = "Database server."
        )]
        public string Server { get; set; }

        [Parameter(
            Mandatory = true,
            Position = 1,
            HelpMessage = "Database name."
        )]
        public string Database { get; set; }

        [Parameter(
            Position = 2,
            HelpMessage = "User name."
        )]
        public string UserId { get; set; }

        [Parameter(
            Position = 3,
            HelpMessage = "User password."
        )]
        public string Password { get; set; }

        [Parameter(
            Position = 4,
            HelpMessage = "Use trusted connection."
        )]
        public SwitchParameter TrustedConnection { get; set; }

        public SqlConnection Connection { get; private set; }

        protected override void BeginProcessing()
        {
            if (TrustedConnection == false && (UserId == null || Password == null))
            {
                throw new ArgumentException("You must use trusted connection or set both UserId and Password!");
            }
        }

        protected override void ProcessRecord()
        {
            Connection = new SqlConnection($"Server={Server};Database={Database};Trusted_Connection={TrustedConnection};User Id={UserId};Password={Password};MultipleActiveResultSets=True;");
            WriteVerbose("New-Connection: Connection created");

            WriteObject(Connection);
        }
    }
}
