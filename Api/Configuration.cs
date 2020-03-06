using System;

namespace Api
{
    public class Configuration
    {
        public static string QuickBooksOnlineClientId => Environment.GetEnvironmentVariable("QuickBooksOnlineClientId");
        public static string QuickBooksOnlineClientSecret => Environment.GetEnvironmentVariable("QuickBooksOnlineClientSecret");
        public static string QuickBooksOnlineRefreshToken => Environment.GetEnvironmentVariable("QuickBooksOnlineRefreshToken");
        public static string RealmId => "9130347957983546";
    }
}
