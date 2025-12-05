using System.Collections.Generic;
using MySql.Data.MySqlClient;
using QuanLySinhVien.Utils;
using QuanLySinhVien.Models;

namespace QuanLySinhVien.Repositories
{
    public class ExamScheduleRepository
    {
        public List<ExamScheduleModel> GetExamScheduleForTeacher(string teacherName)
        {
            var list = new List<ExamScheduleModel>();

            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();

                string sql = @"
                    SELECT 
                        es.exam_id,
                        es.courseClass_id,
                        es.schedule,
                        el.status,
                        COUNT(el.student_id) AS studentCount
                    FROM ExamList el
                    JOIN ExamSchedule es ON el.exam_id = es.exam_id
                    WHERE el.supervisor_name = @name
                    GROUP BY es.exam_id, es.courseClass_id, es.schedule, el.status
                ";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@name", teacherName);

                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            list.Add(new ExamScheduleModel
                            {
                                ExamId = r["exam_id"].ToString(),
                                CourseClassId = r["courseClass_id"].ToString(),
                                Schedule = r["schedule"].ToString(),
                                Status = r["status"].ToString(),
                                StudentCount = r.GetInt32("studentCount")
                            });
                        }
                    }
                }
            }

            return list;
        }
    }
}
