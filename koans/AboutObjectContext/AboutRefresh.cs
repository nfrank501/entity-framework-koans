using System.Data;
using System.Data.Objects;
using System.Linq;
using NUnit.Framework;
using koans.KoansCore;

namespace koans.AboutObjectContext
{
    [TestFixture]
    public class AboutRefresh : AboutBase
    {
        protected override void SetupTestData()
        {
            TruncateTables("Products");

            ExecuteCommands(
                "INSERT INTO Products (Name, Description, Price) " +
                " VALUES (" +
                "  'Widget', " +
                "  'It''s a widget. If you don''t know what it''s for, you might hurt yourself if you bought one.', " +
                "  99.76) ");
        }
 
        [Test]
        public void test_when_two_users_modify_the_same_property_with_client_wins()
        {
            var context1 = GetContext();
            var context2 = GetContext();

            var productOne = context1.Products.First();
            var productTwo = context2.Products.First();

            productOne.Price = 50;
            productTwo.Price = 76;



            var productOneState = context1.ObjectStateManager.GetObjectStateEntry(productOne);

            Assert.AreEqual(EntityState.Modified, productOneState.State);

            var productOneOriginalPrice = productOneState.OriginalValues.GetDecimal(productOneState.OriginalValues.GetOrdinal("Price"));
            var productOneCurrentPrice = productOneState.CurrentValues.GetDecimal(productOneState.CurrentValues.GetOrdinal("Price"));

            Assert.AreEqual(99.76m, productOneOriginalPrice);
            Assert.AreEqual(50m, productOneCurrentPrice);


            context1.SaveChanges();


            Assert.AreEqual(EntityState.Unchanged, productOneState.State);

            productOneOriginalPrice = productOneState.OriginalValues.GetDecimal(productOneState.OriginalValues.GetOrdinal("Price"));
            productOneCurrentPrice = productOneState.CurrentValues.GetDecimal(productOneState.CurrentValues.GetOrdinal("Price"));

            Assert.AreEqual(50m, productOneOriginalPrice);
            Assert.AreEqual(50m, productOneCurrentPrice);





            var productTwoState = context2.ObjectStateManager.GetObjectStateEntry(productTwo);

            Assert.AreEqual(EntityState.Modified, productTwoState.State);

            var productTwoOriginalPrice = productTwoState.OriginalValues.GetDecimal(productTwoState.OriginalValues.GetOrdinal("Price"));
            var productTwoCurrentPrice = productTwoState.CurrentValues.GetDecimal(productTwoState.CurrentValues.GetOrdinal("Price"));

            //Prior to Refresh, the original values are the ones that 
            // orginally came from the database
            Assert.AreEqual(99.76m, productTwoOriginalPrice);
            Assert.AreEqual(76m, productTwoCurrentPrice);


            context2.Refresh(RefreshMode.ClientWins, productTwo);

            Assert.AreEqual(EntityState.Modified, productTwoState.State);

            productTwoOriginalPrice = productTwoState.OriginalValues.GetDecimal(productTwoState.OriginalValues.GetOrdinal("Price"));
            productTwoCurrentPrice = productTwoState.CurrentValues.GetDecimal(productTwoState.CurrentValues.GetOrdinal("Price"));

            //After the refresh, the changes from context 1 have been 
            // pulled in
            Assert.AreEqual(50m, productTwoOriginalPrice);
            Assert.AreEqual(76m, productTwoCurrentPrice);

            context2.SaveChanges();


            Assert.AreEqual(EntityState.Unchanged, productTwoState.State);

            productTwoOriginalPrice = productTwoState.OriginalValues.GetDecimal(productTwoState.OriginalValues.GetOrdinal("Price"));
            productTwoCurrentPrice = productTwoState.CurrentValues.GetDecimal(productTwoState.CurrentValues.GetOrdinal("Price"));

            Assert.AreEqual(76m, productTwoOriginalPrice);
            Assert.AreEqual(76m, productTwoCurrentPrice);


            context1.Dispose();
            context2.Dispose();
        }

        [Test]
        public void test_when_two_users_modify_the_same_property_with_store_wins()
        {
            var context1 = GetContext();
            var context2 = GetContext();

            var productOne = context1.Products.First();
            var productTwo = context2.Products.First();

            productOne.Price = 50;
            productTwo.Price = 76;



            var productOneState = context1.ObjectStateManager.GetObjectStateEntry(productOne);

            Assert.AreEqual(EntityState.Modified, productOneState.State);

            var productOneOriginalPrice = productOneState.OriginalValues.GetDecimal(productOneState.OriginalValues.GetOrdinal("Price"));
            var productOneCurrentPrice = productOneState.CurrentValues.GetDecimal(productOneState.CurrentValues.GetOrdinal("Price"));

            Assert.AreEqual(99.76m, productOneOriginalPrice);
            Assert.AreEqual(50m, productOneCurrentPrice);


            context1.SaveChanges();


            Assert.AreEqual(EntityState.Unchanged, productOneState.State);

            productOneOriginalPrice = productOneState.OriginalValues.GetDecimal(productOneState.OriginalValues.GetOrdinal("Price"));
            productOneCurrentPrice = productOneState.CurrentValues.GetDecimal(productOneState.CurrentValues.GetOrdinal("Price"));

            Assert.AreEqual(50m, productOneOriginalPrice);
            Assert.AreEqual(50m, productOneCurrentPrice);

            
            
            
            var productTwoState = context2.ObjectStateManager.GetObjectStateEntry(productTwo);

            Assert.AreEqual(EntityState.Modified, productTwoState.State);

            var productTwoOriginalPrice = productTwoState.OriginalValues.GetDecimal(productTwoState.OriginalValues.GetOrdinal("Price"));
            var productTwoCurrentPrice = productTwoState.CurrentValues.GetDecimal(productTwoState.CurrentValues.GetOrdinal("Price"));

            //Prior to Refresh, the original values are the ones that 
            // orginally came from the database
            Assert.AreEqual(99.76m, productTwoOriginalPrice);
            Assert.AreEqual(76m, productTwoCurrentPrice);


            context2.Refresh(RefreshMode.StoreWins, productTwo);


            Assert.AreEqual(EntityState.Unchanged, productTwoState.State);

            productTwoOriginalPrice = productTwoState.OriginalValues.GetDecimal(productTwoState.OriginalValues.GetOrdinal("Price"));
            productTwoCurrentPrice = productTwoState.CurrentValues.GetDecimal(productTwoState.CurrentValues.GetOrdinal("Price"));

            //After the refresh, the changes from context 1 have overridden 
            // the changes from context 2
            Assert.AreEqual(50m, productTwoOriginalPrice);
            Assert.AreEqual(50m, productTwoCurrentPrice);


            context1.Dispose();
            context2.Dispose();
        }


        [Test]
        public void test_when_two_users_modify_different_properties_with_client_wins()
        {
            var context1 = GetContext();
            var context2 = GetContext();

            var productOne = context1.Products.First();
            var productTwo = context2.Products.First();

            productOne.Name = "TEST WIDGET";
            productTwo.Price = 76;


            context1.SaveChanges();




            var productTwoState = context2.ObjectStateManager.GetObjectStateEntry(productTwo);

            Assert.AreEqual(EntityState.Modified, productTwoState.State);

            var productTwoOriginalPrice = productTwoState.OriginalValues.GetDecimal(productTwoState.OriginalValues.GetOrdinal("Price"));
            var productTwoCurrentPrice = productTwoState.CurrentValues.GetDecimal(productTwoState.CurrentValues.GetOrdinal("Price"));
            var productTwoOriginalName = productTwoState.OriginalValues.GetString(productTwoState.OriginalValues.GetOrdinal("Name"));
            var productTwoCurrentName = productTwoState.CurrentValues.GetString(productTwoState.CurrentValues.GetOrdinal("Name"));

            //Prior to Refresh, the original values are the ones that 
            // orginally came from the database
            Assert.AreEqual(99.76m, productTwoOriginalPrice);
            Assert.AreEqual(76m, productTwoCurrentPrice);
            Assert.AreEqual("Widget", productTwoOriginalName);
            Assert.AreEqual("Widget", productTwoCurrentName);


            context2.Refresh(RefreshMode.ClientWins, productTwo);

            Assert.AreEqual(EntityState.Modified, productTwoState.State);

            productTwoOriginalPrice = productTwoState.OriginalValues.GetDecimal(productTwoState.OriginalValues.GetOrdinal("Price"));
            productTwoCurrentPrice = productTwoState.CurrentValues.GetDecimal(productTwoState.CurrentValues.GetOrdinal("Price"));
            productTwoOriginalName = productTwoState.OriginalValues.GetString(productTwoState.OriginalValues.GetOrdinal("Name"));
            productTwoCurrentName = productTwoState.CurrentValues.GetString(productTwoState.CurrentValues.GetOrdinal("Name"));

            //With ClientWins, after the refresh, the changes from context 1 have been 
            // pulled in as original values, while the unmodified values originally from 
            // the database are in current
            Assert.AreEqual(99.76m, productTwoOriginalPrice);
            Assert.AreEqual(76m, productTwoCurrentPrice);
            Assert.AreEqual("TEST WIDGET", productTwoOriginalName);
            Assert.AreEqual("Widget", productTwoCurrentName);

            context2.SaveChanges();

            Assert.AreEqual(EntityState.Unchanged, productTwoState.State);

            productTwoOriginalPrice = productTwoState.OriginalValues.GetDecimal(productTwoState.OriginalValues.GetOrdinal("Price"));
            productTwoCurrentPrice = productTwoState.CurrentValues.GetDecimal(productTwoState.CurrentValues.GetOrdinal("Price"));
            productTwoOriginalName = productTwoState.OriginalValues.GetString(productTwoState.OriginalValues.GetOrdinal("Name"));
            productTwoCurrentName = productTwoState.CurrentValues.GetString(productTwoState.CurrentValues.GetOrdinal("Name"));

            // With ClientWins, everything from the client (even unmodified) 
            //  will override what is currently in the database. Thus, even
            //  though context2 had no changes to Name, the value from context1
            //  is overwritten
            Assert.AreEqual(76m, productTwoOriginalPrice);
            Assert.AreEqual(76m, productTwoCurrentPrice);
            Assert.AreEqual("Widget", productTwoOriginalName);
            Assert.AreEqual("Widget", productTwoCurrentName);

            context1.Dispose();
            context2.Dispose();
        }

        [Test]
        public void test_when_two_users_modify_different_properties_with_store_wins()
        {
            var context1 = GetContext();
            var context2 = GetContext();

            var productOne = context1.Products.First();
            var productTwo = context2.Products.First();

            productOne.Name = "TEST WIDGET";
            productTwo.Price = 76;


            context1.SaveChanges();




            var productTwoState = context2.ObjectStateManager.GetObjectStateEntry(productTwo);

            Assert.AreEqual(EntityState.Modified, productTwoState.State);

            var productTwoOriginalPrice = productTwoState.OriginalValues.GetDecimal(productTwoState.OriginalValues.GetOrdinal("Price"));
            var productTwoCurrentPrice = productTwoState.CurrentValues.GetDecimal(productTwoState.CurrentValues.GetOrdinal("Price"));
            var productTwoOriginalName = productTwoState.OriginalValues.GetString(productTwoState.OriginalValues.GetOrdinal("Name"));
            var productTwoCurrentName = productTwoState.CurrentValues.GetString(productTwoState.CurrentValues.GetOrdinal("Name"));

            //Prior to Refresh, the original values are the ones that 
            // orginally came from the database
            Assert.AreEqual(99.76m, productTwoOriginalPrice);
            Assert.AreEqual(76m, productTwoCurrentPrice);
            Assert.AreEqual("Widget", productTwoOriginalName);
            Assert.AreEqual("Widget", productTwoCurrentName);


            context2.Refresh(RefreshMode.StoreWins, productTwo);

            Assert.AreEqual(EntityState.Unchanged, productTwoState.State);

            productTwoOriginalPrice = productTwoState.OriginalValues.GetDecimal(productTwoState.OriginalValues.GetOrdinal("Price"));
            productTwoCurrentPrice = productTwoState.CurrentValues.GetDecimal(productTwoState.CurrentValues.GetOrdinal("Price"));
            productTwoOriginalName = productTwoState.OriginalValues.GetString(productTwoState.OriginalValues.GetOrdinal("Name"));
            productTwoCurrentName = productTwoState.CurrentValues.GetString(productTwoState.CurrentValues.GetOrdinal("Name"));

            //With StoreWins, after the refresh, the changes from context 1 have 
            // completely overwritten the changes we had made to context 2
            Assert.AreEqual(99.76m, productTwoOriginalPrice);
            Assert.AreEqual(99.76m, productTwoCurrentPrice);
            Assert.AreEqual("TEST WIDGET", productTwoOriginalName);
            Assert.AreEqual("TEST WIDGET", productTwoCurrentName);

            context1.Dispose();
            context2.Dispose();
        }


    }
}