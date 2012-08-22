using AboutInheritance;
using NUnit.Framework;
using koans.KoansCore;

namespace koans.AboutInheritance
{
    [TestFixture]
    public class AboutTablePerConcreteClassInheritance : AboutInheritanceBase
    {
        protected override void SetupTestData()
        {
            TruncateTables("Helicopters", "Airplanes");

            ExecuteCommands(
                "INSERT INTO Helicopters (Name, RotorConfiguration) VALUES ('Chinook', 'Tandem') ",
                "INSERT INTO Helicopters (Name, RotorConfiguration) VALUES ('Kamov Ka-27', 'Co-axial') ",
                "INSERT INTO Helicopters (Name, RotorConfiguration) VALUES ('Bell 206', 'Single Main') ");

            ExecuteCommands(
                "INSERT INTO Airplanes (Name, EngineCount) VALUES ('McDonnell Douglas DC-10', 3) ",
                "INSERT INTO Airplanes (Name, EngineCount) VALUES ('Boeing 747', 4) ");
        }

        [Test]
        public void verify_aircraft_class_exists()
        {
            var type = AboutInheritanceAssembly.GetType("AboutInheritance.Aircraft");
            Assert.IsNotNull(type, "Map an aircraft entity from the Aircrafts table in AboutInheritance.edmx");
        }

        [Test]
        public void verify_aircraft_class_is_abstract()
        {
            var type = AboutInheritanceAssembly.GetType("AboutInheritance.Aircraft");
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
            var type = AboutInheritanceAssembly.GetType("AboutInheritance.Helicopter");
            Assert.IsNotNull(type,
                             "Map a helicopter entity that inherits from the aircraft entity in AboutInheritance.edmx");
        }

        [Test]
        public void verify_airplane_class_exists()
        {
            var type = AboutInheritanceAssembly.GetType("AboutInheritance.Airplane");
            Assert.IsNotNull(type,
                             "Map an airplane entity that inherits from the aircraft entity in AboutInheritance.edmx");
        }

        [Test]
        public void verify_helicopter_inherits_from_Aircraft()
        {
            var helicopterType = AboutInheritanceAssembly.GetType("AboutInheritance.Helicopter");
            var aircraftType = AboutInheritanceAssembly.GetType("AboutInheritance.Aircraft");

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
            var airplaneType = AboutInheritanceAssembly.GetType("AboutInheritance.Airplane");
            var aircraftType = AboutInheritanceAssembly.GetType("AboutInheritance.Aircraft");

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

            if (typeof (AboutInheritanceEntities).GetProperty("Helicopters") != null)
            {
                helicopterCount = GetEntityCount("SELECT VALUE h FROM AboutInheritanceEntities.Helicopters AS h");
            }

            Assert.AreEqual(3, helicopterCount, "Setup the discriminator values for helicopters");
        }

        [Test]
        public void verify_the_airplane_discriminator_is_setup_correctly()
        {
            int airplaneCount = 0;


            if (typeof (AboutInheritanceEntities).GetProperty("Airplanes") != null)
            {
                airplaneCount = GetEntityCount("SELECT VALUE a FROM AboutInheritanceEntities.Airplanes AS a");
            }
            Assert.AreEqual(2, airplaneCount, "Setup the discriminator values for airplanes");
        }
    }
}