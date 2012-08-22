using NUnit.Framework;
using AboutInheritance;
using koans.KoansCore;

namespace koans.AboutInheritance
{
    [TestFixture]
    public class AboutTablePerClassHierarchyInheritance : AboutInheritanceBase
    {
        protected override void SetupTestData()
        {
            TruncateTables("Animals");

            ExecuteCommands(
                "INSERT INTO Animals (Type, Name, CatLivesLeft) VALUES ('Cat', 'Fluffy', 9) ",
                "INSERT INTO Animals (Type, Name, CatLivesLeft) VALUES ('Cat', 'Muffin', 8) ");

            ExecuteCommands(
                "INSERT INTO Animals (Type, Name, DogYearsLeft) VALUES ('Dog', 'Fido', 27) ",
                "INSERT INTO Animals (Type, Name, DogYearsLeft) VALUES ('Dog', 'Spot', 16) ",
                "INSERT INTO Animals (Type, Name, DogYearsLeft) VALUES ('Dog', 'Scout', 34) ");
        }


        [Test]
        public void verify_animal_class_exists()
        {
            var type = AboutInheritanceAssembly.GetType("AboutInheritance.Animal");
            Assert.IsNotNull(type, "Map an animal entity from the Animals table in AboutInheritance.edmx");
        }

        [Test]
        public void verify_animal_class_is_abstract()
        {
            var type = AboutInheritanceAssembly.GetType("AboutInheritance.Animal");
            if (type != null)
            {
                Assert.IsTrue(type.IsAbstract,
                              "The animal entity will never be referenced directly and thus needs to be marked abstract in AboutInheritance.edmx");
            }
            else
            {
                Assert.Inconclusive();
            }
        }

        [Test]
        public void verify_dog_class_exists()
        {
            var type = AboutInheritanceAssembly.GetType("AboutInheritance.Dog");
            Assert.IsNotNull(type, "Map a dog entity that inherits from the animal entity in AboutInheritance.edmx");
        }

        [Test]
        public void verify_cat_class_exists()
        {
            var type = AboutInheritanceAssembly.GetType("AboutInheritance.Cat");
            Assert.IsNotNull(type, "Map a cat entity that inherits from the animal entity in AboutInheritance.edmx");
        }

        [Test]
        public void verify_dog_inherits_from_animal()
        {
            var dogType = AboutInheritanceAssembly.GetType("AboutInheritance.Dog");
            var animalType = AboutInheritanceAssembly.GetType("AboutInheritance.Animal");

            if (dogType != null && animalType != null)
            {
                Assert.IsTrue(animalType.IsAssignableFrom(dogType));
            }
            else
            {
                Assert.Inconclusive();
            }
        }

        [Test]
        public void verify_cat_inherits_from_animal()
        {
            var catType = AboutInheritanceAssembly.GetType("AboutInheritance.Cat");
            var animalType = AboutInheritanceAssembly.GetType("AboutInheritance.Animal");

            if (catType != null && animalType != null)
            {
                Assert.IsTrue(animalType.IsAssignableFrom(catType));
            }
            else
            {
                Assert.Inconclusive();
            }
        }

        [Test]
        public void verify_the_cat_discriminator_is_setup_correctly()
        {
            int catCount = 0;

            if (typeof (AboutInheritanceEntities).GetProperty("Animals") != null)
            {
                catCount = GetEntityCount("SELECT VALUE c " +
                                          "FROM OFTYPE(AboutInheritanceEntities.Animals, AboutInheritanceModel.Cat) AS c");
            }
            Assert.AreEqual(2, catCount, "Setup the discriminator values for cats");
        }

        [Test]
        public void verify_the_dog_discriminator_is_setup_correctly()
        {
            int dogCount = 0;

            if (typeof (AboutInheritanceEntities).GetProperty("Animals") != null)
            {
                dogCount = GetEntityCount("SELECT VALUE d " +
                                          "FROM OFTYPE(AboutInheritanceEntities.Animals, AboutInheritanceModel.Dog) AS d");
            }

            Assert.AreEqual(3, dogCount, "Setup the discriminator values for dogs");
        }
    }
}