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
    /// <summary>
    /// <para type="synopsis">Creates new SQL connection bz provided credencies.</para>
    /// <para type="description">Create connection which supports Multiple active result sets.</para>
    /// </summary>
    /// <example>
    ///   <para>Create new trusted connection.</para>
    ///   <code>New-Connection -Server localhost\SQLEXPRESS -Database MyDB -TrustedConnection</code>
    /// </example>
    /// <example>
    ///   <para>Create new connection using username and password.</para>
    ///   <code>New-Connection -Server localhost\SQLEXPRESS -Database MyDB -UserId John -Password MyPass</code>
    /// </example>
    /// <example>
    ///   <para>Create new connection without params specification</para>
    ///   <code>New-Connection localhost\SQLEXPRESS MyDB -TrustedConnection</code>
    /// </example>
    [Cmdlet(VerbsCommon.New, "Connection")]
    [OutputType(typeof(SqlConnection))]
    public class NewConnection : Cmdlet
    {
        /// <summary>
        /// <para type="description">Database server.</para>
        /// </summary>
        [Parameter(
            Mandatory = true,
            Position = 0,
            HelpMessage = "Database server."
        )]
        public string Server { get; set; }

        /// <summary>
        /// <para type="description">Database name.</para>
        /// </summary>
        [Parameter(
            Mandatory = true,
            Position = 1,
            HelpMessage = "Database name."
        )]
        public string Database { get; set; }

        /// <summary>
        /// <para type="description">User name.</para>
        /// </summary>
        [Parameter(
            Position = 2,
            HelpMessage = "User name."
        )]
        public string UserId { get; set; }

        /// <summary>
        /// <para type="description">User password.</para>
        /// </summary>
        [Parameter(
            Position = 3,
            HelpMessage = "User password."
        )]
        public string Password { get; set; }

        /// <summary>
        /// <para type="description">Use trusted connection.</para>
        /// </summary>
        [Parameter(
            Position = 4,
            HelpMessage = "Use trusted connection."
        )]
        public SwitchParameter TrustedConnection { get; set; }

        /// <summary>
        /// <para type="description">Cmdlet output - new created SqlConnection.</para>
        /// </summary>
        public SqlConnection Connection { get; private set; }

        /// <summary>
        /// <para type="description">Begin processing.</para>
        /// </summary>
        protected override void BeginProcessing()
        {
            if (TrustedConnection == false && (UserId == null || Password == null))
            {
                throw new ArgumentException("You must use trusted connection or set both UserId and Password!");
            }
        }

        /// <summary>
        /// <para type="description">Process record.</para>
        /// </summary>
        protected override void ProcessRecord()
        {
            Connection = new SqlConnection($"Server={Server};Database={Database};Trusted_Connection={TrustedConnection};User Id={UserId};Password={Password};MultipleActiveResultSets=True;");
            WriteVerbose("New-Connection: Connection created");

            WriteObject(Connection);
        }
    }
}
