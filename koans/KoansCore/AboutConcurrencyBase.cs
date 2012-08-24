using System.Data.EntityClient;
using System.Data.SqlClient;
using AboutConcurrencyModeFixed;
using AboutConcurrencyModeNone;
using NUnit.Framework;

namespace koans.KoansCore
{
    public abstract class AboutConcurrencyBase : AboutBase
    {

        protected string AboutConcurrencyModeFixedEntitesConnectionString;
        protected string AboutConcurrencyModeNoneEntitesConnectionString;

        protected override string GetConnectionString()
        {
            var builder = new SqlConnectionStringBuilder
                {
                    DataSource = @".\SQLEXPRESS",
                    AttachDBFilename = @"|DataDirectory|\AboutConcurrency.mdf",
                    IntegratedSecurity = true,
                    UserInstance = true,
                    MultipleActiveResultSets = true,
                    ApplicationName = "EntityFramework"
                };

            return builder.ToString();
        }


        [SetUp]
        public void SetupFixture()
        {
            var builder = new EntityConnectionStringBuilder
            {
                ProviderConnectionString = GetConnectionString(),
                Provider = "System.Data.SqlClient",
                Metadata =
                    "res://AboutConcurrency/AboutConcurrencyModeFixed.csdl|res://AboutConcurrency/AboutConcurrencyModeFixed.ssdl|res://AboutConcurrency/AboutConcurrencyModeFixed.msl"
            };


            AboutConcurrencyModeFixedEntitesConnectionString = builder.ToString();

            builder.Metadata =
                "res://AboutConcurrency/AboutConcurrencyModeNone.csdl|res://AboutConcurrency/AboutConcurrencyModeNone.ssdl|res://AboutConcurrency/AboutConcurrencyModeNone.msl";

            AboutConcurrencyModeNoneEntitesConnectionString = builder.ToString();

            InitializationConnection = new SqlConnection(GetConnectionString());
            InitializationConnection.Open();

            SetupTestData();

            InitializationConnection.Close();
            InitializationConnection.Dispose();
        }

        protected void SetupTestData()
        {
            TruncateTables("Products");

            ExecuteCommands(
                "INSERT INTO Products (Name, Description, Price) " +
                " VALUES (" +
                "  'Widget', " +
                "  'It''s a widget. If you don''t know what it''s for, you might hurt yourself if you bought one.', " +
                "  99.76) ");
        }

        protected AboutConcurrencyModeNoneEntities GetAboutConcurrencyModeNoneContext ()
        {
            return new AboutConcurrencyModeNoneEntities(AboutConcurrencyModeNoneEntitesConnectionString);
        }

        protected AboutConcurrencyModeFixedEntities GetAboutConcurrencyModeFixedContext()
        {
            return new AboutConcurrencyModeFixedEntities(AboutConcurrencyModeFixedEntitesConnectionString);
        }


       
    }
}