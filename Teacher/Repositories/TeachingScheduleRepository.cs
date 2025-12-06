using System.Collections.Generic;
using MySql.Data.MySqlClient;
using QuanLySinhVien.Models;
using QuanLySinhVien.Utils;

namespace QuanLySinhVien.Repositories
{
    public class TeachingScheduleRepository
    {
        public List<CourseClassModel> GetTeachingSchedule(string teacherId, string semester, string year)
        {
            var list = new List<CourseClassModel>();

            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();

                string sql = @"
                    SELECT cc.*
                    FROM Teacher_CourseClass tcc
                    JOIN courseClass cc ON tcc.courseClass_id = cc.courseClass_id
                    WHERE tcc.teacher_id = @teacherId
                    AND cc.semester = @semester
                    AND cc.year = @year;
                ";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@teacherId", teacherId);
                    cmd.Parameters.AddWithValue("@semester", semester);
                    cmd.Parameters.AddWithValue("@year", year);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new CourseClassModel
                            {
                                CourseClassId = reader.GetString("courseClass_id"),
                                CourseId = reader.GetString("course_id"),
                                Room = reader.GetString("room"),
                                LearnSchedule = reader.GetString("learnSchedule"),
                                Duration = reader.GetString("duration"),
                                Semester = reader["semester"].ToString(),
                                Year = reader["year"].ToString(),
                                Status = reader.GetString("status"),
                                Capacity = reader.GetInt32("capacity")
                            });
                        }
                    }
                }
            }

            return list;
        }

        public List<string> GetSemesters()
        {
            var list = new List<string>();

            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string sql = "SELECT DISTINCT semester FROM courseClass ORDER BY semester";

                using (var cmd = new MySqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(reader.GetString("semester"));
                    }
                }
            }

            return list;
        }

        public List<string> GetYears()
        {
            var list = new List<string>();

            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string sql = "SELECT DISTINCT year FROM courseClass ORDER BY year";

                using (var cmd = new MySqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(reader.GetString("year"));
                    }
                }
            }

            return list;
        }
    }
}
