using System.Collections.Generic;
using System.Data;
using System.Data.EntityClient;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;
using AboutInheritance;
using NUnit.Framework;

namespace koans.KoansCore
{
    public abstract class AboutInheritanceBase
    {
        private const string AboutInheritanceConnectionString =
    @"data source=.\SQLEXPRESS;attachdbfilename=|DataDirectory|\AboutInheritance.mdf;integrated security=True;user instance=True;multipleactiveresultsets=True;App=EntityFramework";


        protected Assembly AboutInheritanceAssembly;
        protected string AboutInheritanceEntitiesConnectionString;

        private SqlConnection _initializationConnection;

        [TestFixtureSetUp]
        public void SetupFixture()
        {
            AboutInheritanceAssembly = typeof(AboutInheritanceEntities).Assembly;

            var builder = new EntityConnectionStringBuilder
            {
                ProviderConnectionString = AboutInheritanceConnectionString,
                Provider = "System.Data.SqlClient",
                Metadata =
                    "res://AboutInheritance/AboutInheritance.csdl|res://AboutInheritance/AboutInheritance.ssdl|res://AboutInheritance/AboutInheritance.msl"
            };
            AboutInheritanceEntitiesConnectionString = builder.ToString();

            _initializationConnection = new SqlConnection(AboutInheritanceConnectionString);
            _initializationConnection.Open();

            SetupTestData();

            _initializationConnection.Close();
            _initializationConnection.Dispose();
        }

        protected abstract void SetupTestData();



        protected void DeleteFromTables (params string[] tableNames)
        {
            var clearDataBuilder = new StringBuilder();

            foreach (var tableName in tableNames)
            {
                clearDataBuilder.AppendFormat("DELETE FROM {0};", tableName);
            }
            using (var command = _initializationConnection.CreateCommand())
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
            using (var command = _initializationConnection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = clearDataBuilder.ToString();
                command.ExecuteNonQuery();
            }
        }

        protected void ExecuteCommands (params string[] commands)
        {
            var queryBuilder = new StringBuilder();

            foreach (var command in commands)
            {
                queryBuilder.AppendFormat("{0};", command);
            }
            using (var command = _initializationConnection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = queryBuilder.ToString();
                command.ExecuteNonQuery();
            }
        }

        protected int GetEntityCount (string esqlQuery)
        {
            int count = 0;

            using (var connection = new EntityConnection(AboutInheritanceEntitiesConnectionString))
            {
                connection.Open();
                using (var command = new EntityCommand(esqlQuery, connection))
                {
                    using (var reader = command.ExecuteReader(CommandBehavior.SequentialAccess))
                    {
                        while (reader.Read())
                        {
                            count = count + 1;
                        }
                    }
                }
            }

            return count;
        }


    }
}