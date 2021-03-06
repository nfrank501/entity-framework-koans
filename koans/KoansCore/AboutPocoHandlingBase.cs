﻿using System.Data.EntityClient;
using System.Data.SqlClient;
using AboutPocoHandling;
using NUnit.Framework;

namespace koans.KoansCore
{
    public abstract class AboutPocoHandlingBase : AboutBase
    {
        [TestFixtureSetUp]
        public void SetupFixture()
        {
            InitializationConnection = new SqlConnection(GetConnectionString());
            InitializationConnection.Open();

            SetupTestData();

            InitializationConnection.Close();
            InitializationConnection.Dispose();
        }

        protected override void SetupTestData()
        {
            TruncateTables("Products");

            ExecuteCommands(
                "INSERT INTO Products (Name, Price, Active) " +
                " VALUES ('Widget', 99.76, 1) ");
        }

        protected override string GetConnectionString()
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = @".\SQLEXPRESS",
                AttachDBFilename = @"|DataDirectory|\AboutPocoHandling.mdf",
                IntegratedSecurity = true,
                UserInstance = true,
                MultipleActiveResultSets = true,
                ApplicationName = "EntityFramework"
            };

            return builder.ToString();
        }

        protected override string GetEntitiesConnectionString()
        {
            var builder = new EntityConnectionStringBuilder
            {
                ProviderConnectionString = GetConnectionString(),
                Provider = "System.Data.SqlClient",
                Metadata =
                    "res://AboutPocoHandling/AboutPocoHandling.csdl" +
                    "|res://AboutPocoHandling/AboutPocoHandling.ssdl" +
                    "|res://AboutPocoHandling/AboutPocoHandling.msl"
            };
            return builder.ToString();
        }


         protected AboutPocoHandlingContext GetPocoContext()
         {
             return new AboutPocoHandlingContext(GetEntitiesConnectionString());
         }
    }
}