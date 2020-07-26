using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Utils
{
    /// <summary>
    /// <para type="description">Validators.</para>
    /// </summary>
    public static class Validators
    {
        /// <summary>
        /// <para type="description">Validate table name.</para>
        /// <para type="description">Check if table has name, otherwise throw exception.</para>
        /// </summary>
        /// <param name="table">Table to validate.</param>
        /// <param name="paramName">Name of validated parameter.</param>
        /// <exception cref="ArgumentException">On missing TableName attribut.</exception>
        public static void ValidateTableName(DataTable table, string paramName)
        {
            if (table.TableName == null || table.TableName.Length == 0)
            {
                throw new ArgumentException($"{paramName} must have specified TableName!");
            }
        }

        /// <summary>
        /// <para type="description">Validate table primary key.</para>
        /// <para type="description">Check if table has primary key specified, otherwise throw exception.</para>
        /// </summary>
        /// <param name="table">Table to validate.</param>
        /// <param name="paramName">Name of validated parameter.</param>
        /// <exception cref="ArgumentException">On missing Primary key.</exception>
        public static void ValidateTablePrimaryKey(DataTable table, string paramName)
        {
            if (table.PrimaryKey == null || table.PrimaryKey.Length == 0)
            {
                throw new ArgumentException($"{paramName} must have specified Primary key!");
            }
        }
    }
}
