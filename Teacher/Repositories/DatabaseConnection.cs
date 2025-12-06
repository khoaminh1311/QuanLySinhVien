using MySql.Data.MySqlClient;

namespace QuanLySinhVien.Utils
{
    public static class DatabaseConnection
    {
        private static readonly string connectionString =
            "Server=127.0.0.1;Port=3307;Database=doanqlsv;Uid=khoa;Pwd=123;";

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }
    }
}
