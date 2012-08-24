using System.Data;
using System.Data.EntityClient;
using System.Data.SqlClient;
using System.Reflection;
using AboutInheritance;
using NUnit.Framework;

namespace koans.KoansCore
{
    public abstract class AboutInheritanceBase : AboutBase
    {
        protected Assembly AboutInheritanceAssembly;
        protected string AboutInheritanceEntitiesConnectionString;

        protected override string GetConnectionString()
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = @".\SQLEXPRESS",
                AttachDBFilename = @"|DataDirectory|\AboutInheritance.mdf",
                IntegratedSecurity = true,
                UserInstance = true,
                MultipleActiveResultSets = true,
                ApplicationName = "EntityFramework"
            };

            return builder.ToString();
        }



        [TestFixtureSetUp]
        public void SetupFixture()
        {
            AboutInheritanceAssembly = typeof(AboutInheritanceEntities).Assembly;

            var builder = new EntityConnectionStringBuilder
            {
                ProviderConnectionString = GetConnectionString(),
                Provider = "System.Data.SqlClient",
                Metadata =
                    "res://AboutInheritance/AboutInheritance.csdl|res://AboutInheritance/AboutInheritance.ssdl|res://AboutInheritance/AboutInheritance.msl"
            };
            AboutInheritanceEntitiesConnectionString = builder.ToString();

            InitializationConnection = new SqlConnection(GetConnectionString());
            InitializationConnection.Open();

            SetupTestData();

            InitializationConnection.Close();
            InitializationConnection.Dispose();
        }

        protected int GetEntityCount(string esqlQuery)
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