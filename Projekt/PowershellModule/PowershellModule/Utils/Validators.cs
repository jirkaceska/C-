using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Utils
{
    public static class Validators
    {
        public static void ValidateTable(DataTable table, string paramName)
        {
            if (table.TableName == null || table.TableName.Length == 0)
            {
                throw new ArgumentException($"{paramName} must have specified TableName!");
            }
        }
    }
}
