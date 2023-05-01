using System;
using dotenv.net;
using dotenv.net.Utilities;

namespace kalbestore_be.utils
{
    public class dbconfig
    {
        public static string ConnectionString { get; }

        static dbconfig()
        {
            // Get values from .env file or use default values
            DotEnv.Load();
            string host = EnvReader.GetStringValue("DB_HOST");
            string username = EnvReader.GetStringValue("DB_USERNAME");
            string password = EnvReader.GetStringValue("DB_PASSWORD");
            string database = EnvReader.GetStringValue("DB_DATABASE");

            ConnectionString = $"Host={host};Username={username};Password={password};Database={database}";
        }
    }
}
