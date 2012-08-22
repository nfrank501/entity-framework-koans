using System.Data;
using System.Data.EntityClient;
using System.Data.SqlClient;
using System.Reflection;
using NUnit.Framework;
using AboutInheritance;

namespace koans.AboutInheritance
{
    [TestFixture]
    public class AboutTablePerClassHierarchyInheritance
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
                    command.CommandText = "TRUNCATE TABLE Animals";
                    command.ExecuteNonQuery();
                }

                //insert cats
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText =
                        "INSERT INTO Animals (Type, Name, CatLivesLeft) VALUES ('Cat', 'Fluffy', 9) " +
                        "INSERT INTO Animals (Type, Name, CatLivesLeft) VALUES ('Cat', 'Muffin', 8) ";
                    command.ExecuteNonQuery();
                }

                //insert dogs
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = "INSERT INTO Animals (Type, Name, DogYearsLeft) VALUES ('Dog', 'Fido', 27) " +
                                          "INSERT INTO Animals (Type, Name, DogYearsLeft) VALUES ('Dog', 'Spot', 16) " +
                                          "INSERT INTO Animals (Type, Name, DogYearsLeft) VALUES ('Dog', 'Scout', 34) ";
                    command.ExecuteNonQuery();
                }
            }
        }


        [Test]
        public void verify_animal_class_exists()
        {
            var type = _aboutInheritanceAssembly.GetType("AboutInheritance.Animal");
            Assert.IsNotNull(type, "Map an animal entity from the Animals table in AboutInheritance.edmx");
        }

        [Test]
        public void verify_animal_class_is_abstract()
        {
            var type = _aboutInheritanceAssembly.GetType("AboutInheritance.Animal");
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
            var type = _aboutInheritanceAssembly.GetType("AboutInheritance.Dog");
            Assert.IsNotNull(type, "Map a dog entity that inherits from the animal entity in AboutInheritance.edmx");
        }

        [Test]
        public void verify_cat_class_exists()
        {
            var type = _aboutInheritanceAssembly.GetType("AboutInheritance.Cat");
            Assert.IsNotNull(type, "Map a cat entity that inherits from the animal entity in AboutInheritance.edmx");
        }

        [Test]
        public void verify_dog_inherits_from_animal()
        {
            var dogType = _aboutInheritanceAssembly.GetType("AboutInheritance.Dog");
            var animalType = _aboutInheritanceAssembly.GetType("AboutInheritance.Animal");

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
            var catType = _aboutInheritanceAssembly.GetType("AboutInheritance.Cat");
            var animalType = _aboutInheritanceAssembly.GetType("AboutInheritance.Animal");

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


            if (typeof(AboutInheritanceEntities).GetProperty("Animals") != null)
            {

                using (var connection = new EntityConnection(_aboutInheritanceEntitiesConnectionString))
                {
                    connection.Open();
                    var query =
                        "SELECT VALUE c FROM OFTYPE(AboutInheritanceEntities.Animals, AboutInheritanceModel.Cat) AS c";
                    using (var command = new EntityCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader(CommandBehavior.SequentialAccess))
                        {
                            while (reader.Read())
                            {
                                catCount = catCount + 1;
                            }
                        }
                    }
                }
            }
            Assert.AreEqual(2, catCount, "Setup the discriminator values for cats");
        }

        [Test]
        public void verify_the_dog_discriminator_is_setup_correctly()
        {
            int dogCount = 0;

            if (typeof(AboutInheritanceEntities).GetProperty("Animals") != null)
            {
                using (var connection = new EntityConnection(_aboutInheritanceEntitiesConnectionString))
                {
                    connection.Open();
                    var query = "SELECT VALUE d FROM OFTYPE(AboutInheritanceEntities.Animals, AboutInheritanceModel.Dog) AS d";
                    using (var command = new EntityCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader(CommandBehavior.SequentialAccess))
                        {
                            while (reader.Read())
                            {
                                dogCount = dogCount + 1;
                            }
                        }
                    }
                }
            }

            Assert.AreEqual(3, dogCount, "Setup the discriminator values for dogs");
        }
    }
}