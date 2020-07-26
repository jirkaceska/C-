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

namespace Database
{
    enum Options
    {
        Yes,
        No
    }

    [Cmdlet(VerbsData.ConvertTo, "DataTable")]
    public class ConvertToDataTable : PSCmdlet
    {
        [Parameter(
            Mandatory = true,
            Position = 0,
            HelpMessage = "Name of table."
        )]
        public string TableName { get; set; }

        [Parameter(
            Position = 1,
            HelpMessage = "Table to populate. If not provided, new is created and its schema is derived from first row."
        )]
        public DataTable Table { get; set; }

        [Parameter(
            Position = 2,
            HelpMessage = "Column indices which are considered to be in primary key. Numbered from 0. Default is first (0)."
        )]
        public int[] PrimaryKeyColumns { get; set; } = { 0 };

        [Parameter(
            Mandatory = true,
            ValueFromPipeline = true,
            Position = 3,
            HelpMessage = "Rows to convert to DataTable."
        )]
        public PSObject Row { get; set; }
        
        public bool IsInitialized { get; private set; } = true;

        protected override void BeginProcessing()
        {
            if (Table == null)
            {
                Table = new DataTable();
                IsInitialized = false;
            }
        }

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

            foreach (var property in Row.SelectKeyValuePair(PropertyToColumn))
            {
                var type = property.Value;
                if (type == typeof(DBNull))
                {
                    var choice = Host.UI.PromptForChoice(
                        $"Column {property.Key} at first row contains NULL and therefore cannot be determined its type.",
                        "Please choose type:",
                        choices,
                        Constants.DBTypes.Count - 2
                    );
                    type = Constants.DBTypes[choice].Value;
                }
                try
                {
                    Table.Columns.Add(property.Key, type);
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
                Host.UI.WriteErrorLine($"Invalid cast type at {Row.SelectKeyValuePair(PropertyToTuple).Select(FormatExtension.FormatKeyValuePair).ConcatAndWrap()}");
                Host.UI.WriteErrorLine($"Cannot convert a <{property.Value}> to a {expectedType}.");
            }
            else if (e is ConstraintException)
            {
                Host.UI.WriteErrorLine($"Constraint violation at {Row.SelectKeyValuePair(PropertyToTuple).Select(FormatExtension.FormatKeyValuePair).ConcatAndWrap()}");
                Host.UI.WriteErrorLine($"Column {property.Name} is constrained to be unique. Value <{property.Value}> is already inserted.");
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
                if (property.Value.Equals(""))
                {
                    property.Value = DBNull.Value;
                }
                row[property.Name] = property.Value;
                shouldSkipLine = false;
                return true;
            }
            catch (Exception e) when (e is ArgumentException || e is ConstraintException)
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

        private KeyValuePair<string, Type> PropertyToColumn(PSPropertyInfo property)
        {
            return new KeyValuePair<string, Type>(property.Name, Type.GetType(property.TypeNameOfValue));
        }
    }
}
