using System.Data;
using System.Data.EntityClient;
using System.Data.SqlClient;
using System.Text;
using Koans.Data;
using NUnit.Framework;

namespace koans.KoansCore
{
    public abstract class AboutBase
    {
        protected SqlConnection InitializationConnection { get; set; }

        [SetUp]
        public void SetupFixture()
        {
            InitializationConnection = new SqlConnection(GetConnectionString());
            InitializationConnection.Open();

            SetupTestData();

            InitializationConnection.Close();
            InitializationConnection.Dispose();
        }

        protected abstract void SetupTestData();


        protected virtual string GetConnectionString()
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = @".\SQLEXPRESS",
                AttachDBFilename = @"|DataDirectory|\Koans.Data.mdf",
                IntegratedSecurity = true,
                UserInstance = true,
                MultipleActiveResultSets = true,
                ApplicationName = "EntityFramework"
            };

            return builder.ToString();
        }

        protected virtual string GetEntitiesConnectionString()
        {
            var builder = new EntityConnectionStringBuilder
                {
                    ProviderConnectionString = GetConnectionString(),
                    Provider = "System.Data.SqlClient",
                    Metadata =
                        "res://Koans.Data/KoansModel.csdl" +
                        "|res://Koans.Data/KoansModel.ssdl" +
                        "|res://Koans.Data/KoansModel.msl"
                };
            return builder.ToString();
        }

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

        protected KoansEntities GetContext()
        {
            return new KoansEntities(GetEntitiesConnectionString());
        }
        
    }
}