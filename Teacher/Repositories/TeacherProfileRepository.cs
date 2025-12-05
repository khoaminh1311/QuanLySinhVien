using MySql.Data.MySqlClient;
using QuanLySinhVien.Models;
using QuanLySinhVien.Utils;

namespace QuanLySinhVien.Repositories
{
    public class TeacherProfileRepository
    {
        public TeacherInfoModel GetTeacherProfile(string teacherId)
        {
            var model = new TeacherInfoModel();

            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();

                string sql = @"SELECT teacher_id, full_name, gender, email, phone, address, faculty, avatar_path
                               FROM teacher WHERE teacher_id = @id";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", teacherId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            model.TeacherId = reader.GetString("teacher_id");
                            model.FullName = reader.GetString("full_name");
                            model.Gender = reader.GetString("gender");
                            model.Email = reader.GetString("email");
                            model.Phone = reader.GetString("phone");
                            model.Address = reader.GetString("address");
                            model.Faculty = reader.GetString("faculty");
                            model.AvatarPath = reader["avatar_path"]?.ToString();

                        }
                    }
                }
            }
            return model;
        }
    }
}
