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
using System.Management.Automation.Host;
using System.Globalization;
using System.ComponentModel;

namespace Database
{
    enum Options
    {
        Yes,
        No
    }

    /// <summary>
    /// <para type="synopsis">Convert enumeration of PSObject to DataTable.</para>
    /// <para type="description">Needed for processing by most of this module cmdlets.</para>
    /// <para type="description">Cmdlet provide type control (if appropriate DataTable is provided as parameter) with possibility to fix invalid values.</para>
    /// </summary>
    /// <example>
    ///   <para>Add processes to DataTable. Do not forget to filter only useful properties.</para>
    ///   <code>Get-Process | select -Property Id, ProcessName | ConvertTo-DataTable Processes</code>
    /// </example>
    /// <example>
    ///   <para>Import table from CSV to DataTable with name Table.</para>
    ///   <code>Import-Csv -Path .\test.csv | ConvertTo-DataTable Table</code>
    /// </example>
    [Cmdlet(VerbsData.ConvertTo, "DataTable")]
    [OutputType(typeof(DataTable))]
    public class ConvertToDataTable : PSCmdlet
    {
        /// <summary>
        /// <para type="description">Name of table.</para>
        /// </summary>
        [Parameter(
            Mandatory = true,
            Position = 0,
            HelpMessage = "Name of table."
        )]
        public string TableName { get; set; }

        /// <summary>
        /// <para type="description">Table to populate. If not provided, new is created and its schema is derived from first row.</para>
        /// </summary>
        [Parameter(
            Position = 1,
            HelpMessage = "Table to populate. If not provided, new is created and its schema is derived from first row."
        )]
        public DataTable Table { get; set; }

        /// <summary>
        /// <para type="description">Column indices which are considered to be in primary key. Numbered from 0. Default is first (0).</para>
        /// </summary>
        [Parameter(
            Position = 2,
            HelpMessage = "Column indices which are considered to be in primary key. Numbered from 0. Default is first (0)."
        )]
        public int[] PrimaryKeyColumns { get; set; } = { 0 };

        /// <summary>
        /// <para type="description">Rows to convert to DataTable.</para>
        /// </summary>
        [Parameter(
            Mandatory = true,
            ValueFromPipeline = true,
            Position = 3,
            HelpMessage = "Rows to convert to DataTable."
        )]
        public PSObject Row { get; set; }

        /// <summary>
        /// <para type="description">Is table initialized.</para>
        /// </summary>
        public bool IsInitialized { get; private set; } = true;

        /// <summary>
        /// <para type="description">Begin processing.</para>
        /// </summary>
        protected override void BeginProcessing()
        {
            if (Table == null)
            {
                Table = new DataTable();
                IsInitialized = false;
            }
        }

        /// <summary>
        /// <para type="description">Process record.</para>
        /// </summary>
        protected override void ProcessRecord()
        {
            if (!IsInitialized)
            {
                Initialize();
            }
            bool dataRowValid = false;

            do
            {
                try
                {
                    var row = CreateTableRow();
                    if (row != null)
                    {
                        Table.Rows.Add(row);
                    }
                    dataRowValid = true;
                }
                catch (ConstraintException e)
                {
                    foreach (var key in Table.PrimaryKey)
                    {
                        dataRowValid = TryChangePropertyValue(Row.Properties[key.ColumnName], e);
                    }
                }
            } while (!dataRowValid);
        }

        /// <summary>
        /// <para type="description">End processing.</para>
        /// </summary>
        protected override void EndProcessing()
        {
            WriteObject(Table);
        }

        private void Initialize()
        {
            Table.TableName = TableName;
            InitializeColumns();
            Table.PrimaryKey = PrimaryKeyColumns.Select(i => Table.Columns[i]).ToArray();
            IsInitialized = true;
        }

        private DataRow CreateTableRow()
        {
            var row = Table.NewRow();
            var columns = Table.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
            if (PopulateDataRow(row, columns))
            {
                return row;
            }
            return null;
        }

        private bool PopulateDataRow(DataRow row, IEnumerable<string> columns)
        {
            bool shouldSkipLine = true;

            foreach (var column in columns)
            {
                if (!Table.Columns.Contains(column))
                {
                    continue;
                }

                while (!TryToInsert(row, Row.Properties[column], out shouldSkipLine)) ;
                if (shouldSkipLine)
                {
                    break;
                }
            }
            return !shouldSkipLine;
        }

        private void InitializeColumns()
        {
            var choices = Constants.DBTypes.SelectT(
                choice => new ChoiceDescription(
                    choice.Key.Split(',')[0],
                    $"{choice.Key.Replace("&", "")} ({choice.Value.Name})"
                )
            );

            foreach (var property in Row.Properties)
            {
                var type = Type.GetType(property.TypeNameOfValue);
                if (type == null)
                {
                    continue;
                }
                if (type == typeof(DBNull))
                {
                    var choice = Host.UI.PromptForChoice(
                        $"Column {property.Name} at first row contains NULL and therefore cannot be determined its type.",
                        "Please choose type:",
                        choices,
                        Constants.DBTypes.Count - 2
                    );
                    type = Constants.DBTypes[choice].Value;
                }
                try
                {
                    if (Constants.DBTypes.Any(tuple => tuple.Value == type))
                    {
                        WriteVerbose($"Column {property.Name} is added to database");
                        Table.Columns.Add(property.Name, type);
                    }
                    else
                    {
                        WriteWarning($"Column {property.Name} will be skipped because its type cannot be saved to database.");
                    }
                }
                catch (DuplicateNameException e)
                {
                    this.WriteError(e, ErrorCategory.InvalidOperation);
                    throw;
                }
            }
        }

        private Options TryAgain(PSPropertyInfo property, Exception e)
        {
            if (e is ArgumentException)
            {
                var expectedType = Table.Columns[property.Name].DataType.Name;
                Host.UI.WriteErrorLine($"Invalid cast type at {Row.SelectKeyValuePair(Table, PropertyToTuple).Select(FormatExtension.FormatKeyValuePair).ConcatAndWrap()}");
                Host.UI.WriteErrorLine($"Cannot convert a <{property.Value}> to a {expectedType}.");
            }
            else if (e is ConstraintException)
            {
                Host.UI.WriteErrorLine($"Constraint violation at {Row.SelectKeyValuePair(Table, PropertyToTuple).Select(FormatExtension.FormatKeyValuePair).ConcatAndWrap()}");
                Host.UI.WriteErrorLine($"Column {property.Name} is constrained to be unique. Value <{property.Value}> is already inserted.");
            }
            else if (e is GetValueInvocationException)
            {
                Host.UI.WriteErrorLine($"Cannot read row attribute at {Row.SelectKeyValuePair(Table, PropertyToTuple).Select(FormatExtension.FormatKeyValuePair).ConcatAndWrap()}");
                return Options.No;
            }
            var options = Enum.GetValues(typeof(Options)).Cast<object>();

            return (Options)Host.UI.PromptForChoice(
                    $"Value <{property.Value}> cannot be inserted into column {property.Name}.",
                    "Do you want to manually reenter new value?",
                    options.SelectT(option => new ChoiceDescription($"&{option}")),
                    (int)Options.Yes
                );
        }

        private bool TryToInsert(DataRow row, PSPropertyInfo property, out bool shouldSkipLine)
        {
            try
            {
                if (property.Value == null || property.Value.Equals(""))
                {
                    property.Value = DBNull.Value;
                }
                if (property.Value.GetType() == typeof(PSObject))
                {
                    property.Value = ((PSObject)property.Value).BaseObject;
                }
                row[property.Name] = property.Value;

                shouldSkipLine = false;
                return true;
            }
            catch (Exception e) when (e is ArgumentException || e is ConstraintException || e is GetValueInvocationException)
            {
                shouldSkipLine = TryChangePropertyValue(property, e);
                return shouldSkipLine;
            }
        }

        private bool TryChangePropertyValue(PSPropertyInfo property, Exception e)
        {
            var shouldSkipLine = TryAgain(property, e) == Options.No;
            if (shouldSkipLine)
            {
                WriteWarning("This line will be skipped.");
            }
            else
            {
                Host.UI.WriteLine("Enter new value:");
                var value = Host.UI.ReadLine();
                if (value == "")
                {
                    property.Value = DBNull.Value;
                }
                else
                {
                    property.Value = value;
                }
            }
            return shouldSkipLine;
        }

        private KeyValuePair<string, object> PropertyToTuple(PSPropertyInfo property)
        {
            return new KeyValuePair<string, object>(property.Name, property.Value);
        }
    }
}
