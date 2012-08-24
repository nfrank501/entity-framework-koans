using System.Data;
using System.Data.Objects;
using System.Linq;
using NUnit.Framework;
using koans.KoansCore;

namespace koans.AboutConcurrency
{
    [TestFixture]
    public class AboutConcurrencyModeFixed : AboutConcurrencyBase
    {
        [Test]
        public void test_when_two_users_modify_the_same_fields_on_the_same_record_default()
        {
            var context1 = GetContext();
            var context2 = GetContext();

            var product1 = context1.FixedProducts.First();
            var product2 = context2.FixedProducts.First();

            product1.Price = 50;
            product2.Price = 76;

            context1.SaveChanges();

            Assert.Throws<OptimisticConcurrencyException>(() => context2.SaveChanges());
           
            context1.Dispose();
            context2.Dispose();
        }

        [Test]
        public void test_when_two_users_modify_the_same_fields_on_the_same_record_call_refresh()
        {
            var context1 = GetContext();
            var context2 = GetContext();

            var product1 = context1.FixedProducts.First();
            var product2 = context2.FixedProducts.First();

            product1.Price = 50;
            product2.Price = 76;

            context1.SaveChanges();

            //Refresh the entity 
            context2.Refresh(RefreshMode.ClientWins, product2);

            // Save is successful
            context2.SaveChanges();

            context1.Dispose();
            context2.Dispose();
        }




    }
}