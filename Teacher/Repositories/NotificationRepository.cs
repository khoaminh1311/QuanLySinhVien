using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using QuanLySinhVien.Models;
using QuanLySinhVien.Utils;

namespace QuanLySinhVien.Repositories
{
    public class NotificationRepository
    {
        public List<NotificationModel> GetNotifications(string role)
        {
            var list = new List<NotificationModel>();

            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();

                string sql = @"
                SELECT title, content, date, target
                FROM notification
                WHERE target = @role OR target = 'Tất cả'
                ORDER BY date DESC";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@role", role);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new NotificationModel
                            {
                                Title = reader.GetString("title"),
                                Content = reader.GetString("content"),
                                Date = reader.GetDateTime("date"),
                                Target = reader.GetString("target")
                            });
                        }
                    }
                }
            }

            return list;
        }
    }
}
