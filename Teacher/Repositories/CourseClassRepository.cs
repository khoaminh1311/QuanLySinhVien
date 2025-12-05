using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using QuanLySinhVien.Models;
using QuanLySinhVien.Utils;

namespace QuanLySinhVien.Repositories
{
    public class CourseClassRepository
    {
        public List<CourseClassModel> GetCourseClassesByTeacher(string teacherId)
        {
            var list = new List<CourseClassModel>();

            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();

                string sql = @"
                    SELECT cc.courseClass_id, cc.course_id, cc.room, cc.learnSchedule, 
                           cc.duration, cc.semester, cc.year, cc.status, cc.capacity
                    FROM Teacher_CourseClass tcc
                    JOIN CourseClass cc ON tcc.courseClass_id = cc.courseClass_id
                    WHERE tcc.teacher_id = @teacherId";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@teacherId", teacherId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new CourseClassModel
                            {
                                CourseClassId = reader["courseClass_id"].ToString(),
                                CourseId = reader["course_id"].ToString(),
                                Room = reader["room"].ToString(),
                                LearnSchedule = reader["learnSchedule"].ToString(),
                                Duration = reader["duration"].ToString(),
                                Semester = Convert.ToInt32(reader["semester"]),
                                Year = Convert.ToInt32(reader["year"]),
                                Status = reader["status"].ToString(),
                                Capacity = Convert.ToInt32(reader["capacity"])
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

        public List<CourseClassModel> GetClassesBySemester(string teacherId, string semester)
        {
            var list = new List<CourseClassModel>();

            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();

                string sql = @"
            SELECT cc.courseClass_id, cc.course_id
            FROM courseClass cc
            JOIN teacher_courseclass tc ON cc.courseClass_id = tc.courseClass_id
            WHERE tc.teacher_id = @teacherId AND cc.semester = @semester";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@teacherId", teacherId);
                    cmd.Parameters.AddWithValue("@semester", semester);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new CourseClassModel
                            {
                                CourseClassId = reader.GetString("courseClass_id"),
                                CourseId = reader.GetString("course_id")
                            });
                        }
                    }
                }
            }

            return list;
        }

    }
}
