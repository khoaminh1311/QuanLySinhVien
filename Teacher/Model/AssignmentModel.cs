using System;

namespace QuanLySinhVien.Models
{
    public class AssignmentModel
    {
        public int Id { get; set; }
        public string CourseClassId { get; set; }
        public string TeacherId { get; set; }
        public string Content { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? SubmitDate { get; set; }
    }
}
