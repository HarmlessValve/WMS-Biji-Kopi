using System;
using Npgsql;

namespace CoffeeWMS.Data
{
    public static class DatabaseHelper
    {
        // PENTING: File ini adalah contoh (example).
        // Buatlah salinan file ini dengan nama 'DatabaseHelper.cs' (tanpa .example)
        // lalu isikan password dan kredensial database Anda yang sesungguhnya di dalamnya.
        
        private const string ConnectionString = "Host=localhost;Port=5432;Database=smg_kopi;Username=postgres;Password=YOUR_SECRET_PASSWORD";

        public static NpgsqlConnection GetConnection()
        {
            var conn = new NpgsqlConnection(ConnectionString);
            return conn;
        }
    }
}
