using System.Collections.Generic;
using MySql.Data.MySqlClient;
using QuanLySinhVien.Models;
using QuanLySinhVien.Utils;

namespace QuanLySinhVien.Repositories
{
    public class AdvisorClassRepository
    {
        public List<AdvisorStudentModel> GetAdvisorStudents(string teacherId)
        {
            var list = new List<AdvisorStudentModel>();

            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();

                string sql = @"
                    SELECT 
                        ac.student_id,
                        s.full_name,
                        s.gender,
                        s.email,
                        ac.khoa
                    FROM Advisor_Class ac
                    JOIN Student s ON s.student_id = ac.student_id
                    WHERE ac.teacher_id = @teacherId
                    ORDER BY s.full_name;
                ";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@teacherId", teacherId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new AdvisorStudentModel
                            {
                                StudentId = reader["student_id"].ToString(),
                                FullName = reader["full_name"].ToString(),
                                Gender = reader["gender"].ToString(),
                                Email = reader["email"].ToString(),
                                Khoa = reader.GetInt32(reader.GetOrdinal("khoa"))
                            });
                        }
                    }
                }
            }

            return list;
        }
    }
}
