using System.Data;
using System.Linq;
using AboutPocoHandling;
using NUnit.Framework;
using koans.KoansCore;

namespace koans.AboutConcurrency
{
    [TestFixture]
    public class AboutDetectChanges : AboutPocoHandlingBase
    {
        [Test]
        public void test_when_modifying_a_poco_entity()
        {
            Product entity;

            using (var context = GetContext())
            {
                entity = context.Products.First();

                var state = context.ObjectStateManager.GetObjectStateEntry(entity);

                Assert.AreEqual(EntityState.Unchanged, state.State);

                entity.Price = 100m;

                Assert.AreEqual(EntityState.Unchanged, state.State);

                var modifiedProperties = state.GetModifiedProperties();
                Assert.IsFalse(modifiedProperties.Any(x => x == "Price"));
            }
        }

        [Test]
        public void test_when_modifying_a_poco_entity_and_calling_detect_changes()
        {
            Product entity;

            using (var context = GetContext())
            {
                entity = context.Products.First();

                var state = context.ObjectStateManager.GetObjectStateEntry(entity);

                Assert.AreEqual(EntityState.Unchanged, state.State);

                entity.Price = 100m;

                context.DetectChanges();

                Assert.AreEqual(EntityState.Modified, state.State);

                var modifiedProperties = state.GetModifiedProperties();
                Assert.IsTrue(modifiedProperties.Any(x => x == "Price"));
            }
        }

    }
}