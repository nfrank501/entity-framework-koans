using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace koans.KoansCore
{
    public abstract class AboutBase
    {
        protected abstract string GetConnectionString();

        protected SqlConnection InitializationConnection { get; set; }


        protected void DeleteFromTables(params string[] tableNames)
        {
            var clearDataBuilder = new StringBuilder();

            foreach (var tableName in tableNames)
            {
                clearDataBuilder.AppendFormat("DELETE FROM {0};", tableName);
            }
            using (var command = InitializationConnection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = clearDataBuilder.ToString();
                command.ExecuteNonQuery();
            }
        }

        protected void TruncateTables(params string[] tableNames)
        {
            var clearDataBuilder = new StringBuilder();

            foreach (var tableName in tableNames)
            {
                clearDataBuilder.AppendFormat("TRUNCATE TABLE {0};", tableName);
            }
            using (var command = InitializationConnection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = clearDataBuilder.ToString();
                command.ExecuteNonQuery();
            }
        }

        protected void ExecuteCommands(params string[] commands)
        {
            var queryBuilder = new StringBuilder();

            foreach (var command in commands)
            {
                queryBuilder.AppendFormat("{0};", command);
            }
            using (var command = InitializationConnection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = queryBuilder.ToString();
                command.ExecuteNonQuery();
            }
        }

        
    }
}