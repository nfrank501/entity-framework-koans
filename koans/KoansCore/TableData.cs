using System.Collections.Generic;

namespace koans.KoansCore
{
    public class TableData
    {
        
        public string TableName { get; set; }

        private IList<Dictionary<string, object>> _initialData;
        public IList<Dictionary<string, object>> InitialData
        {
            get { return _initialData ?? (_initialData = new List<Dictionary<string, object>>()); }
            set { _initialData = value; }
        }


        public ClearDataMethod ClearDataMethod { get; set; }
    }

    public enum ClearDataMethod
    {
        Unknown = 0, 
        Truncate, 
        Delete
    }
}