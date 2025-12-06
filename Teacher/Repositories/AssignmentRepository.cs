using System.Collections.Generic;
using System.Windows;
using MySql.Data.MySqlClient;
using QuanLySinhVien.Models;
using QuanLySinhVien.Utils;

public class AssignmentRepository
{
    public void InsertAssignment(AssignmentModel a)
    {
        using (var conn = DatabaseConnection.GetConnection())
        {
            conn.Open();

            string sql = @"INSERT INTO Assignment
                           (courseClass_id, teacher_id, content, start_date, end_date)
                           VALUES (@cid, @tid, @content, @start, @end)";

            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@cid", a.CourseClassId);
                cmd.Parameters.AddWithValue("@tid", a.TeacherId);
                cmd.Parameters.AddWithValue("@content", a.Content);
                cmd.Parameters.AddWithValue("@start", a.StartDate);
                cmd.Parameters.AddWithValue("@end", a.EndDate);

                cmd.ExecuteNonQuery();
            }
        }
    }

    public List<AssignmentModel> GetAssignments(string teacherId)
    {
        var list = new List<AssignmentModel>();

        using (var conn = DatabaseConnection.GetConnection())
        {
            conn.Open();
            string sql = @"SELECT * FROM assignment 
                           WHERE teacher_id = @t";

            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@t", teacherId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new AssignmentModel
                        {
                            Id = reader.GetInt32("id"),
                            CourseClassId = reader.GetString("courseClass_id"),
                            TeacherId = reader.GetString("teacher_id"),
                            Content = reader.GetString("content"),
                            StartDate = reader.GetDateTime("start_date"),
                            EndDate = reader.GetDateTime("end_date")
                        });
                    }
                }
            }
        }
        return list;
    }

    public void UpdateAssignment(AssignmentModel a)
    {
        using (var conn = DatabaseConnection.GetConnection())
        {
            conn.Open();
            string sql = @"UPDATE assignment 
                           SET content = @c, 
                               start_date = @s, 
                               end_date = @e 
                           WHERE id = @id";

            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", a.Id);
                cmd.Parameters.AddWithValue("@c", a.Content);
                cmd.Parameters.AddWithValue("@s", a.StartDate);
                cmd.Parameters.AddWithValue("@e", a.EndDate);
                cmd.ExecuteNonQuery();
            }
        }
    }

    public void DeleteAssignment(int id)
    {
        using (var conn = DatabaseConnection.GetConnection())
        {
            conn.Open();
            string sql = @"DELETE FROM assignment WHERE id = @id";

            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }
    }
    public List<AssignmentModel> GetAssignmentsByTeacher(string teacherId)
    {
        var list = new List<AssignmentModel>();

        using (var conn = DatabaseConnection.GetConnection())
        {
            conn.Open();
            string sql = @"SELECT id, content 
                       FROM assignment 
                       WHERE teacher_id = @t";

            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@t", teacherId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new AssignmentModel
                        {
                            Id = reader.GetInt32("id"),
                            Content = reader.GetString("content")
                        });
                    }
                }
            }
        }
        return list;
    }

}
