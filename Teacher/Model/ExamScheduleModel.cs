using System;

namespace QuanLySinhVien.Models
{
    public class ExamScheduleModel
    {
        public string ExamId { get; set; }
        public string CourseClassId { get; set; }
        public string Schedule { get; set; }
        public string Status { get; set; }
        public int StudentCount { get; set; }
    }
}
