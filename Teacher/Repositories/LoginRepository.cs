using System;
using System.Windows;
using MySql.Data.MySqlClient;
using QuanLySinhVien.Utils;

namespace QuanLySinhVien.Repositories
{
    public class LoginRepository
    {
        public bool CheckLogin(string username, string password, string role)
        {
            try {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string sql = "";

                    if (role == "teacher")
                    {
                        sql = @"SELECT COUNT(*) 
                            FROM teacher 
                            WHERE teacher_id = @user 
                            AND password = @pass";
                    }
                    else if (role == "student")
                    {
                        sql = @"SELECT COUNT(*) 
                            FROM student 
                            WHERE student_id = @user 
                            AND password = @pass";
                    }
                    else if (role == "admin")
                    {
                        sql = @"SELECT COUNT(*) 
                            FROM admin 
                            WHERE admin_id = @user 
                            AND password = @pass";
                    }
                    else
                    {
                        MessageBox.Show("Role này chưa hỗ trợ!");
                        return false;
                    }
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@user", username);
                        cmd.Parameters.AddWithValue("@pass", password);

                        var count = Convert.ToInt32(cmd.ExecuteScalar());
                        return count > 0;
                    }
                }
            }
            catch(MySqlException ex)
            {
                MessageBox.Show("Lỗi kết nối cơ sở dữ liệu: " + ex.Message);
                return false;
            }
        }

        public string GetName(string userId, string role)
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();

                //string sql = "SELECT full_name FROM teacher WHERE teacher_id = @id";
                string sql = "";
                // CHỌN BẢNG TUỲ THEO ROLE
                if (role == "teacher")
                    sql = "SELECT full_name FROM Teacher WHERE teacher_id = @id";
                else if (role == "student")
                    sql = "SELECT full_name FROM Student WHERE student_id = @id";
                else if (role == "admin")
                    sql = "SELECT admin_name FROM Admin WHERE admin_id = @id";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", userId);

                    var result = cmd.ExecuteScalar();
                    return result?.ToString() ?? "";
                }
                //    if (role == "teacher" && userId == "10358299")
                //    return "Nguyễn Nam Hùng (Mock)";

                //if (role == "student")
                //    return "Sinh viên Mock";

                //if (role == "admin")
                //    return "Admin Mock";

                //return "";
            }
        }

    }
}
