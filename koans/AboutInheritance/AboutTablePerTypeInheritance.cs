using System.Data;
using System.Data.EntityClient;
using System.Data.SqlClient;
using System.Reflection;
using AboutInheritance;
using NUnit.Framework;

namespace koans
{
    [TestFixture]
    public class AboutTablePerTypeInheritance
    {
        private const string AboutInheritanceConnectionString =
            @"data source=.\SQLEXPRESS;attachdbfilename=|DataDirectory|\AboutInheritance.mdf;integrated security=True;user instance=True;multipleactiveresultsets=True;App=EntityFramework";


        private Assembly _aboutInheritanceAssembly;
        private string _aboutInheritanceEntitiesConnectionString;


        [TestFixtureSetUp]
        public void SetupFixture()
        {
            _aboutInheritanceAssembly = typeof (AboutInheritanceEntities).Assembly;

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
                    command.CommandText = "DELETE FROM Cars; DELETE FROM Trucks; DELETE FROM Vehicles; ";
                    command.ExecuteNonQuery();
                }

                //insert records into vehicles table
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText =
                        "INSERT INTO Vehicles (Name) VALUES ('Ford Mustang') " +
                        "INSERT INTO Vehicles (Name) VALUES ('Mini Hatch') " +
                        "INSERT INTO Vehicles (Name) VALUES ('Zastava Yugo') " +
                        "INSERT INTO Vehicles (Name) VALUES ('Mack Granite') " +
                        "INSERT INTO Vehicles (Name) VALUES ('Peterbilt Model 379') ";
                    command.ExecuteNonQuery();
                }

                //insert records into cars table
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText =
                        "INSERT INTO Cars (Id, GasTankGallons) SELECT Id, 100 FROM Vehicles WHERE Name = 'Ford Mustang' " +
                        "INSERT INTO Cars (Id, GasTankGallons) SELECT Id, 50 FROM Vehicles WHERE Name = 'Mini Hatch' " +
                        "INSERT INTO Cars (Id, GasTankGallons) SELECT Id, 25 FROM Vehicles WHERE Name = 'Zastava Yugo'";
                    command.ExecuteNonQuery();
                }

                //insert records into trucks table
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText =
                        "INSERT INTO Trucks (Id, HaulingCapacityTons) SELECT Id, 500 FROM Vehicles WHERE Name = 'Mack Granite' " +
                        "INSERT INTO Trucks (Id, HaulingCapacityTons) SELECT Id, 550 FROM Vehicles WHERE Name = 'Peterbilt Model 379' ";
                    command.ExecuteNonQuery();
                }
            }
        }

        [Test]
        public void verify_vehicle_class_exists()
        {
            var type = _aboutInheritanceAssembly.GetType("AboutInheritance.Vehicle");
            Assert.IsNotNull(type, "Map a vehicle entity from the Vehicles table in AboutInheritance.edmx");
        }

        [Test]
        public void verify_vehicle_class_is_abstract()
        {
            var type = _aboutInheritanceAssembly.GetType("AboutInheritance.Vehicle");
            if (type != null)
            {
                Assert.IsTrue(type.IsAbstract,
                              "The vehicle entity will never be referenced directly and thus needs to be marked abstract in AboutInheritance.edmx");
            }
            else
            {
                Assert.Inconclusive();
            }
        }

        [Test]
        public void verify_car_class_exists()
        {
            var type = _aboutInheritanceAssembly.GetType("AboutInheritance.Car");
            Assert.IsNotNull(type, "Map a car entity that inherits from the vehicle entity in AboutInheritance.edmx");
        }

        [Test]
        public void verify_truck_class_exists()
        {
            var type = _aboutInheritanceAssembly.GetType("AboutInheritance.Truck");
            Assert.IsNotNull(type, "Map a truck entity that inherits from the vehicle entity in AboutInheritance.edmx");
        }

        [Test]
        public void verify_car_inherits_from_vehicle()
        {
            var carType = _aboutInheritanceAssembly.GetType("AboutInheritance.Car");
            var vehicleType = _aboutInheritanceAssembly.GetType("AboutInheritance.Vehicle");

            if (carType != null && vehicleType != null)
            {
                Assert.IsTrue(vehicleType.IsAssignableFrom(carType));
            }
            else
            {
                Assert.Inconclusive();
            }
        }

        [Test]
        public void verify_truck_inherits_from_vehicle()
        {
            var truckType = _aboutInheritanceAssembly.GetType("AboutInheritance.Truck");
            var vehicleType = _aboutInheritanceAssembly.GetType("AboutInheritance.Vehicle");

            if (truckType != null && vehicleType != null)
            {
                Assert.IsTrue(vehicleType.IsAssignableFrom(truckType));
            }
            else
            {
                Assert.Inconclusive();
            }
        }

        [Test]
        public void verify_the_car_discriminator_is_setup_correctly()
        {
            int carCount = 0;

            if (typeof (AboutInheritanceEntities).GetProperty("Vehicles") != null)
            {
                using (var connection = new EntityConnection(_aboutInheritanceEntitiesConnectionString))
                {
                    connection.Open();
                    var query =
                        "SELECT VALUE d FROM OFTYPE(AboutInheritanceEntities.Vehicles, AboutInheritanceModel.Car) AS d";
                    using (var command = new EntityCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader(CommandBehavior.SequentialAccess))
                        {
                            while (reader.Read())
                            {
                                carCount = carCount + 1;
                            }
                        }
                    }
                }
            }

            Assert.AreEqual(3, carCount, "Setup the discriminator values for Cars");
        }

        [Test]
        public void verify_the_truck_discriminator_is_setup_correctly()
        {
            int truckCount = 0;


            if (typeof (AboutInheritanceEntities).GetProperty("Vehicles") != null)
            {
                using (var connection = new EntityConnection(_aboutInheritanceEntitiesConnectionString))
                {
                    connection.Open();
                    var query =
                        "SELECT VALUE c FROM OFTYPE(AboutInheritanceEntities.Vehicles, AboutInheritanceModel.Truck) AS c";
                    using (var command = new EntityCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader(CommandBehavior.SequentialAccess))
                        {
                            while (reader.Read())
                            {
                                truckCount = truckCount + 1;
                            }
                        }
                    }
                }
            }
            Assert.AreEqual(2, truckCount, "Setup the discriminator values for Trucks");
        }
    }
}