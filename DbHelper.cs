using Npgsql;

namespace Concurrent_Booking
{
    public class DbHelper
    {
        private String dbConString;
        public DbHelper()
        {
            NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder();
            builder.Host = "localhost";
            builder.Port = 5432;
            builder.Username = "postgres";
            builder.Password = "mysecretpassword";
            builder.Database = "Movie_Management";
            dbConString = builder.ToString();
        }
        public async Task<NpgsqlConnection> getOpenConnectionAsync()
        {
            NpgsqlConnection con = new NpgsqlConnection(dbConString);
            await con.OpenAsync();
            return con;
        }
    }
}
