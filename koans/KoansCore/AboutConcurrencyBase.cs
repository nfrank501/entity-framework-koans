namespace koans.KoansCore
{
    public abstract class AboutConcurrencyBase : AboutBase
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




       
    }
}