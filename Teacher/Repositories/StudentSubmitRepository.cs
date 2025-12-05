using System.Collections.Generic;
using MySql.Data.MySqlClient;
using QuanLySinhVien.Utils;

public class StudentSubmitRepository
{
    public List<StudentSubmitModel> GetSubmissions(int assignmentId)
    {
        var list = new List<StudentSubmitModel>();

        using (var conn = DatabaseConnection.GetConnection())
        {
            conn.Open();

            string sql = @"
                SELECT ss.*, st.full_name, st.email
                FROM student_submit ss
                JOIN student st ON ss.student_id = st.student_id
                WHERE ss.assignment_id = @aid";

            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@aid", assignmentId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new StudentSubmitModel
                        {
                            Id = reader.GetInt32("id"),
                            AssignmentId = reader.GetInt32("assignment_id"),
                            StudentId = reader.GetString("student_id"),
                            SubmitContent = reader.GetString("submit_content"),
                            SubmitDate = reader.GetDateTime("submit_date"),
                            FullName = reader.GetString("full_name"),
                            Email = reader.GetString("email")
                        });
                    }
                }
            }
        }
        return list;
    }
}
