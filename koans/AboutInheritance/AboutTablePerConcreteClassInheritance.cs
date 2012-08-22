using System.Data;
using System.Data.EntityClient;
using System.Data.SqlClient;
using System.Reflection;
using AboutInheritance;
using NUnit.Framework;

namespace koans.AboutInheritance
{
    [TestFixture]
    public class AboutTablePerConcreteClassInheritance
    {
        private const string AboutInheritanceConnectionString =
    @"data source=.\SQLEXPRESS;attachdbfilename=|DataDirectory|\AboutInheritance.mdf;integrated security=True;user instance=True;multipleactiveresultsets=True;App=EntityFramework";


        private Assembly _aboutInheritanceAssembly;
        private string _aboutInheritanceEntitiesConnectionString;


        [TestFixtureSetUp]
        public void SetupFixture()
        {
            _aboutInheritanceAssembly = typeof(AboutInheritanceEntities).Assembly;

            var builder = new EntityConnectionStringBuilder
            {
                ProviderConnectionString = AboutInheritanceConnectionString,
                Provider = "System.Data.SqlClient",
                Metadata =
                    "res://AboutInheritance/AboutInheritance.csdl|res://AboutInheritance/AboutInheritance.ssdl|res://AboutInheritance/AboutInheritance.msl"
            };
            _aboutInheritanceEntitiesConnectionString = builder.ToString();


            //initialize data
            using (var connection = new SqlConnection(AboutInheritanceConnectionString))
            {
                connection.Open();

                // wipe out any existing data
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = "TRUNCATE TABLE Helicopters; TRUNCATE TABLE Airplanes; ";
                    command.ExecuteNonQuery();
                }

                //insert records into Helicopters table
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText =
                        "INSERT INTO Helicopters (Name, RotorConfiguration) VALUES ('Chinook', 'Tandem') " +
                        "INSERT INTO Helicopters (Name, RotorConfiguration) VALUES ('Kamov Ka-27', 'Co-axial') " +
                        "INSERT INTO Helicopters (Name, RotorConfiguration) VALUES ('Bell 206', 'Single Main') ";
                    command.ExecuteNonQuery();
                }

                //insert records into Airplanes table
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText =
                        "INSERT INTO Airplanes (Name, EngineCount) VALUES ('McDonnell Douglas DC-10', 3) " +
                        "INSERT INTO Airplanes (Name, EngineCount) VALUES ('Boeing 747', 4) ";
                    command.ExecuteNonQuery();
                }
            }
        }

        [Test]
        public void verify_aircraft_class_exists()
        {
            var type = _aboutInheritanceAssembly.GetType("AboutInheritance.Aircraft");
            Assert.IsNotNull(type, "Map an aircraft entity from the Aircrafts table in AboutInheritance.edmx");
        }

        [Test]
        public void verify_aircraft_class_is_abstract()
        {
            var type = _aboutInheritanceAssembly.GetType("AboutInheritance.Aircraft");
            if (type != null)
            {
                Assert.IsTrue(type.IsAbstract,
                              "The aircraft entity will never be referenced directly and thus needs to be marked abstract in AboutInheritance.edmx");
            }
            else
            {
                Assert.Inconclusive();
            }
        }

        [Test]
        public void verify_helicopter_class_exists()
        {
            var type = _aboutInheritanceAssembly.GetType("AboutInheritance.Helicopter");
            Assert.IsNotNull(type, "Map a helicopter entity that inherits from the aircraft entity in AboutInheritance.edmx");
        }

        [Test]
        public void verify_airplane_class_exists()
        {
            var type = _aboutInheritanceAssembly.GetType("AboutInheritance.Airplane");
            Assert.IsNotNull(type, "Map an airplane entity that inherits from the aircraft entity in AboutInheritance.edmx");
        }

        [Test]
        public void verify_helicopter_inherits_from_Aircraft()
        {
            var helicopterType = _aboutInheritanceAssembly.GetType("AboutInheritance.Helicopter");
            var aircraftType = _aboutInheritanceAssembly.GetType("AboutInheritance.Aircraft");

            if (helicopterType != null && aircraftType != null)
            {
                Assert.IsTrue(aircraftType.IsAssignableFrom(helicopterType));
            }
            else
            {
                Assert.Inconclusive();
            }
        }

        [Test]
        public void verify_airplane_inherits_from_Aircraft()
        {
            var airplaneType = _aboutInheritanceAssembly.GetType("AboutInheritance.Airplane");
            var aircraftType = _aboutInheritanceAssembly.GetType("AboutInheritance.Aircraft");

            if (airplaneType != null && aircraftType != null)
            {
                Assert.IsTrue(aircraftType.IsAssignableFrom(airplaneType));
            }
            else
            {
                Assert.Inconclusive();
            }
        }

        [Test]
        public void verify_the_helicopter_discriminator_is_setup_correctly()
        {
            int helicopterCount = 0;

            if (typeof(AboutInheritanceEntities).GetProperty("Helicopters") != null)
            {
                using (var connection = new EntityConnection(_aboutInheritanceEntitiesConnectionString))
                {
                    connection.Open();
                    var query =
                        "SELECT VALUE h FROM AboutInheritanceEntities.Helicopters AS h";
                    using (var command = new EntityCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader(CommandBehavior.SequentialAccess))
                        {
                            while (reader.Read())
                            {
                                helicopterCount = helicopterCount + 1;
                            }
                        }
                    }
                }
            }

            Assert.AreEqual(3, helicopterCount, "Setup the discriminator values for helicopters");
        }

        [Test]
        public void verify_the_airplane_discriminator_is_setup_correctly()
        {
            int airplaneCount = 0;


            if (typeof(AboutInheritanceEntities).GetProperty("Airplanes") != null)
            {
                using (var connection = new EntityConnection(_aboutInheritanceEntitiesConnectionString))
                {
                    connection.Open();
                    var query =
                        "SELECT VALUE a FROM AboutInheritanceEntities.Airplanes AS a";
                    using (var command = new EntityCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader(CommandBehavior.SequentialAccess))
                        {
                            while (reader.Read())
                            {
                                airplaneCount = airplaneCount + 1;
                            }
                        }
                    }
                }
            }
            Assert.AreEqual(2, airplaneCount, "Setup the discriminator values for airplanes");
        }
 
    }
}