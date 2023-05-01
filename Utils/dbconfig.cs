using System;
using DotNetEnv;

namespace kalbestore_be.utils
{
    public class dbconfig
    {
        public static string ConnectionString { get; }

        static dbconfig()
        {
            Env.Load(); // Load values from .env file

            // Get values from .env file or use default values
            string host = Env.GetString("DB_HOST", "localhost");
            string username = Env.GetString("DB_USERNAME", "postgres");
            string password = Env.GetString("DB_PASSWORD", "postgres");
            string database = Env.GetString("DB_DATABASE", "kalbe");

            ConnectionString = $"Host={host};Username={username};Password={password};Database={database}";
        }
    }
}
