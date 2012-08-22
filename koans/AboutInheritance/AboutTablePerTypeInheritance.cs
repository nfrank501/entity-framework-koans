using AboutInheritance;
using NUnit.Framework;
using koans.KoansCore;

namespace koans
{
    [TestFixture]
    public class AboutTablePerTypeInheritance : AboutInheritanceBase
    {
        protected override void SetupTestData()
        {
            DeleteFromTables("Cars", "Trucks", "Vehicles");

            ExecuteCommands(
                "INSERT INTO Vehicles (Name) VALUES ('Ford Mustang') ",
                "INSERT INTO Vehicles (Name) VALUES ('Mini Hatch') ",
                "INSERT INTO Vehicles (Name) VALUES ('Zastava Yugo') ",
                "INSERT INTO Vehicles (Name) VALUES ('Mack Granite') ",
                "INSERT INTO Vehicles (Name) VALUES ('Peterbilt Model 379') ");

            ExecuteCommands(
                "INSERT INTO Cars (Id, GasTankGallons) SELECT Id, 100 FROM Vehicles WHERE Name = 'Ford Mustang' ",
                "INSERT INTO Cars (Id, GasTankGallons) SELECT Id, 50 FROM Vehicles WHERE Name = 'Mini Hatch' ",
                "INSERT INTO Cars (Id, GasTankGallons) SELECT Id, 25 FROM Vehicles WHERE Name = 'Zastava Yugo'");

            ExecuteCommands(
                "INSERT INTO Trucks (Id, HaulingCapacityTons) SELECT Id, 500 FROM Vehicles WHERE Name = 'Mack Granite' ",
                "INSERT INTO Trucks (Id, HaulingCapacityTons) SELECT Id, 550 FROM Vehicles WHERE Name = 'Peterbilt Model 379' ");
        }

        [Test]
        public void verify_vehicle_class_exists()
        {
            var type = AboutInheritanceAssembly.GetType("AboutInheritance.Vehicle");
            Assert.IsNotNull(type, "Map a vehicle entity from the Vehicles table in AboutInheritance.edmx");
        }

        [Test]
        public void verify_vehicle_class_is_abstract()
        {
            var type = AboutInheritanceAssembly.GetType("AboutInheritance.Vehicle");
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
            var type = AboutInheritanceAssembly.GetType("AboutInheritance.Car");
            Assert.IsNotNull(type, "Map a car entity that inherits from the vehicle entity in AboutInheritance.edmx");
        }

        [Test]
        public void verify_truck_class_exists()
        {
            var type = AboutInheritanceAssembly.GetType("AboutInheritance.Truck");
            Assert.IsNotNull(type, "Map a truck entity that inherits from the vehicle entity in AboutInheritance.edmx");
        }

        [Test]
        public void verify_car_inherits_from_vehicle()
        {
            var carType = AboutInheritanceAssembly.GetType("AboutInheritance.Car");
            var vehicleType = AboutInheritanceAssembly.GetType("AboutInheritance.Vehicle");

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
            var truckType = AboutInheritanceAssembly.GetType("AboutInheritance.Truck");
            var vehicleType = AboutInheritanceAssembly.GetType("AboutInheritance.Vehicle");

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
                carCount = GetEntityCount("SELECT VALUE c " +
                                          "FROM OFTYPE(AboutInheritanceEntities.Vehicles, AboutInheritanceModel.Car) AS c");
            }

            Assert.AreEqual(3, carCount, "Setup the discriminator values for Cars");
        }

        [Test]
        public void verify_the_truck_discriminator_is_setup_correctly()
        {
            int truckCount = 0;

            if (typeof (AboutInheritanceEntities).GetProperty("Vehicles") != null)
            {
                truckCount = GetEntityCount("SELECT VALUE t " +
                                            "FROM OFTYPE(AboutInheritanceEntities.Vehicles, AboutInheritanceModel.Truck) AS t");
            }
            Assert.AreEqual(2, truckCount, "Setup the discriminator values for Trucks");
        }
    }
}