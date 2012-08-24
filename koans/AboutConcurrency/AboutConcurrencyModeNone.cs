using System.Data;
using System.Linq;
using NUnit.Framework;
using koans.KoansCore;

namespace koans.AboutConcurrency
{
    [TestFixture]
    public class AboutConcurrencyModeNone : AboutConcurrencyBase
    {

        




        [Test]
        public void test_when_two_users_modify_the_same_fields_on_the_same_record_default ()
        {
            var context1 = GetAboutConcurrencyModeNoneContext();
            var context2 = GetAboutConcurrencyModeNoneContext();

            var product1 = context1.Products.First();
            var product2 = context2.Products.First();

            product1.Price = 50;
            product2.Price = 76;

            context1.SaveChanges();
            context2.SaveChanges();

            var context3 = GetAboutConcurrencyModeNoneContext();
            var product3 = context3.Products.First();

            //Last in wins
            Assert.AreEqual(76m, product3.Price);

            
            context1.Dispose();
            context2.Dispose();
            context3.Dispose();
        }

        [Test]
        public void test_when_two_users_modify_the_same_fields_on_the_same_record_call_detect_changes()
        {
            var context1 = GetAboutConcurrencyModeNoneContext();
            var context2 = GetAboutConcurrencyModeNoneContext();

            var product1 = context1.Products.First();
            var product2 = context2.Products.First();

            product1.Price = 50;
            product2.Price = 76;

            context1.SaveChanges();


            context2.DetectChanges();

            var state2 = context2.ObjectStateManager.GetObjectStateEntry(product2);

            Assert.AreEqual(EntityState.Modified, state2.State);
            var fieldOrdinal2 = state2.OriginalValues.GetOrdinal("Price");
            var originalValue2 = state2.OriginalValues.GetDecimal(fieldOrdinal2);

            //since the same field is modified, detect changes doesn't pull in the 
            // context1 modifications
            Assert.AreEqual(99.76m, originalValue2);

            context2.SaveChanges();

            Assert.AreEqual(EntityState.Unchanged, state2.State);


            var context3 = GetAboutConcurrencyModeNoneContext();
            var product3 = context3.Products.First();

            //Last in wins
            Assert.AreEqual(76m, product3.Price);

            context1.Dispose();
            context2.Dispose();
            context3.Dispose();
        }


        [Test]
        public void test_when_two_users_modify_different_fields_on_the_same_record()
        {
            var context1 = GetAboutConcurrencyModeNoneContext();
            var context2 = GetAboutConcurrencyModeNoneContext();

            var product1 = context1.Products.First();
            var product2 = context2.Products.First();

            product1.Description = "TEST";
            product2.Price = 76;

            context1.DetectChanges();

            var state1 = context1.ObjectStateManager.GetObjectStateEntry(product1);

            Assert.AreEqual(EntityState.Modified, state1.State);
            var fieldOrdinal1 = state1.OriginalValues.GetOrdinal("Price");
            var originalValue1 = state1.OriginalValues.GetDecimal(fieldOrdinal1);

            Assert.AreEqual(99.76m, originalValue1);

            context1.SaveChanges();

            Assert.AreEqual(EntityState.Unchanged, state1.State);

            context2.DetectChanges();

            var state2 = context2.ObjectStateManager.GetObjectStateEntry(product2);

            Assert.AreEqual(EntityState.Modified, state2.State);
            var priceFieldOrdinal = state2.OriginalValues.GetOrdinal("Price");
            var priceOriginalValue = state2.OriginalValues.GetDecimal(priceFieldOrdinal);
            var descriptionFieldOrdinal = state2.OriginalValues.GetOrdinal("Description");
            var descriptionOriginalValue = state2.OriginalValues.GetString(descriptionFieldOrdinal);

            Assert.AreEqual(99.76m, priceOriginalValue);
            //Assert.AreEqual("TEST", descriptionOriginalValue);

            context2.SaveChanges();

            Assert.AreEqual(EntityState.Unchanged, state2.State);

            var context3 = GetAboutConcurrencyModeNoneContext();
            var product3 = context3.Products.First();

            Assert.AreEqual(76m, product3.Price);
            Assert.AreEqual("TEST", product3.Description);


            context1.Dispose();
            context2.Dispose();
        }



        [Test]
        public void test_when_two_users_modify_different_fields_on_the_same_record_with_detect_changes()
        {
            var context1 = GetAboutConcurrencyModeNoneContext();
            var context2 = GetAboutConcurrencyModeNoneContext();

            var product1 = context1.Products.First();
            var product2 = context2.Products.First();

            product1.Description = "TEST";
            product2.Price = 76;

            context1.DetectChanges();

            var state1 = context1.ObjectStateManager.GetObjectStateEntry(product1);

            Assert.AreEqual(EntityState.Modified, state1.State);
            var fieldOrdinal1 = state1.OriginalValues.GetOrdinal("Price");
            var originalValue1 = state1.OriginalValues.GetDecimal(fieldOrdinal1);

            Assert.AreEqual(99.76m, originalValue1);

            context1.SaveChanges();

            Assert.AreEqual(EntityState.Unchanged, state1.State);

            context2.DetectChanges();

            var state2 = context2.ObjectStateManager.GetObjectStateEntry(product2);
            var modifiedProperties = state2.GetModifiedProperties();

            Assert.AreEqual(EntityState.Modified, state2.State);
            var priceFieldOrdinal = state2.OriginalValues.GetOrdinal("Price");
            var priceOriginalValue = state2.OriginalValues.GetDecimal(priceFieldOrdinal);
            var descriptionFieldOrdinal = state2.OriginalValues.GetOrdinal("Description");
            var descriptionOriginalValue = state2.OriginalValues.GetString(descriptionFieldOrdinal);

            Assert.AreEqual(99.76m, priceOriginalValue);
            //Assert.AreEqual("TEST", descriptionOriginalValue);

            context2.SaveChanges();

            Assert.AreEqual(EntityState.Unchanged, state2.State);

            var context3 = GetAboutConcurrencyModeNoneContext();
            var product3 = context3.Products.First();

            Assert.AreEqual(76m, product3.Price);
            Assert.AreEqual("TEST", product3.Description);


            context1.Dispose();
            context2.Dispose();
        }

    }

}