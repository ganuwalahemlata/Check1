using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Web;

namespace KontinuityCRM.Helpers
{
    public static class GenericDataAccess
    {
        /// <summary>
        /// Execute Select Command
        /// </summary>
        /// <param name="command">Db Command</param>
        /// <returns></returns>
        public static DataTable ExecuteSelectCommand(DbCommand command)
        {
            // The DataTable to be returned 
            DataTable table;
            // Execute the command making sure the connection gets closed in the end
            try
            {
                // Open the data connection 
                command.Connection.Open();
                // Execute the command and save the results in a DataTable
                DbDataReader reader = command.ExecuteReader();
                table = new DataTable();
                table.Load(reader);

                // Close the reader 
                reader.Close();
            }
            catch (Exception ex)
            {
                //Utilities.LogError(ex);
                throw;
            }
            finally
            {
                // Close the connection
                command.Connection.Close();
            }
            return table;
        }
    }
}