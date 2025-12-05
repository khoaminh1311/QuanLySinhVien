using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using QuanLySinhVien.Models;
using QuanLySinhVien.Utils;

namespace QuanLySinhVien.Repositories
{
    public class GradeRepository
    {
        public List<GradeRecord> GetStudentGrades(string courseClassId, string teacherId)
        {
            var list = new List<GradeRecord>();

            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();

                string sql = @"
            SELECT 
                s.student_id,
                s.full_name,
                g.attendance,
                g.mid_term,
                g.final_term,
                g.total
            FROM student_courseclass sc
            JOIN student s ON sc.student_id = s.student_id
            LEFT JOIN grade g 
                ON g.student_id = s.student_id
                AND g.courseClass_id = sc.courseClass_id
                AND g.teacher_id = @teacherId
            WHERE sc.courseClass_id = @classId";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@classId", courseClassId);
                    cmd.Parameters.AddWithValue("@teacherId", teacherId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            float attendance = reader["attendance"] == DBNull.Value ? 0 : Convert.ToSingle(reader["attendance"]);
                            float midterm = reader["mid_term"] == DBNull.Value ? 0 : Convert.ToSingle(reader["mid_term"]);
                            float finalterm = reader["final_term"] == DBNull.Value ? 0 : Convert.ToSingle(reader["final_term"]);
                            float total = reader["total"] == DBNull.Value ? 0 : Convert.ToSingle(reader["total"]);

                            list.Add(new GradeRecord
                            {
                                StudentId = reader.GetString("student_id"),
                                FullName = reader.GetString("full_name"),
                                Attendance = attendance,
                                MidTerm = midterm,
                                FinalTerm = finalterm,
                                Total = total,
                                CourseClassId = courseClassId,
                                TeacherId = teacherId
                            });
                        }
                    }
                }
            }

            return list;
        }

        public void UpdateGrade(GradeRecord grade)
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();

                string sql = @"
                    INSERT INTO grade (student_id, courseClass_id, teacher_id, attendance, mid_term, final_term, total)
                    VALUES (@student_id, @courseClass_id, @teacher_id, @attendance, @midterm, @finalterm, @total)
                    ON DUPLICATE KEY UPDATE
                    attendance = VALUES(attendance),
                    mid_term   = VALUES(mid_term),
                    final_term = VALUES(final_term),
                    total= VALUES(total);";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@student_id", grade.StudentId);
                    cmd.Parameters.AddWithValue("@courseClass_id", grade.CourseClassId);
                    cmd.Parameters.AddWithValue("@teacher_id", grade.TeacherId);
                    cmd.Parameters.AddWithValue("@attendance", grade.Attendance);
                    cmd.Parameters.AddWithValue("@midterm", grade.MidTerm);
                    cmd.Parameters.AddWithValue("@finalterm", grade.FinalTerm);
                    cmd.Parameters.AddWithValue("@total", grade.Total);

                    cmd.ExecuteNonQuery();
                }
            }
        }


        public class GradeRecord
        {
            public string StudentId { get; set; }
            public string FullName { get; set; }

            // Các thuộc tính dùng để hiển thị trên DataGrid
            public string AttendanceText { get; set; }
            public string MidTermText { get; set; }
            public string FinalTermText { get; set; }
            public string TotalText { get; set; }

            // Các thuộc tính dùng để lưu vào DB (được Parse từ Text)
            public float Attendance { get; set; }
            public float MidTerm { get; set; }
            public float FinalTerm { get; set; }
            public float Total { get; set; }

            public string CourseClassId { get; set; }
            public string TeacherId { get; set; }
        }
    }

}
