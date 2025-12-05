using System;
using MySql.Data.MySqlClient;
using QuanLySinhVien.Models;
using QuanLySinhVien.Utils;

namespace QuanLySinhVien.Repositories
{
    public class TeacherRepository
    {
        public string GetTeacherName(int teacherId)
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string sql = "SELECT full_name FROM Teacher WHERE teacher_id = @id";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", teacherId);
                    var result = cmd.ExecuteScalar();
                    return result?.ToString() ?? "Không tìm thấy";
                }
            }
        }

        public int GetTotalClasses(int teacherId)
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string sql = "SELECT COUNT(*) FROM Teacher_CourseClass WHERE teacher_id = @id";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", teacherId);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public bool UpdateTeacher(TeacherInfoModel t)
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();

                string sql = @"
            UPDATE Teacher SET
                full_name = @name,
                gender = @gender,
                email = @email,
                address = @address,
                faculty = @faculty
            WHERE teacher_id = @id
        ";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@name", t.FullName);
                    cmd.Parameters.AddWithValue("@gender", t.Gender);
                    cmd.Parameters.AddWithValue("@email", t.Email);
                    cmd.Parameters.AddWithValue("@address", t.Address);
                    cmd.Parameters.AddWithValue("@id", t.TeacherId);
                    cmd.Parameters.AddWithValue("@faculty", t.Faculty);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public void UpdateAvatar(string teacherId, string avatarPath)
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();

                string sql = @"UPDATE teacher 
                       SET avatar_path = @path 
                       WHERE teacher_id = @id";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@path", avatarPath);
                    cmd.Parameters.AddWithValue("@id", teacherId);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
