using System;
using System.Web;

namespace Cities.Data
{
    public static class Database
    {
        private const string DATACONTEXT_ITEMS_KEY = "DataContextKey";

        [ThreadStatic]
        private static DatabaseEntities _context = null;

        /// <summary>        
        /// Private property to store the DataContext in the HttpContext.Current.Items or thread local storage        
        /// </summary>
        /// <value>Model data context</value>
        public static DatabaseEntities Context
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    if (_context == null)
                        _context = NewContext();
                    return _context;
                }
                else
                {
                    if (!HttpContext.Current.Items.Contains(DATACONTEXT_ITEMS_KEY))
                    {
                        HttpContext.Current.Items[DATACONTEXT_ITEMS_KEY] = new DatabaseEntities();
                    }
                    return (DatabaseEntities)HttpContext.Current.Items[DATACONTEXT_ITEMS_KEY];
                }
            }
        }

        public static DatabaseEntities NewContext()
        {
            return new DatabaseEntities();
        }
    }
}
