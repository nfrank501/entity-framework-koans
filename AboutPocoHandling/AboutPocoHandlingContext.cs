using System.Data.Objects;

namespace AboutPocoHandling
{
    public class AboutPocoHandlingContext : ObjectContext
    {
        public AboutPocoHandlingContext(string connectionString) : base(connectionString)
        {
        }

        private ObjectSet<Product> _products;
        public ObjectSet<Product>  Products
        {
            get { return _products ?? (_products = base.CreateObjectSet<Product>()); }
        }
    }
}