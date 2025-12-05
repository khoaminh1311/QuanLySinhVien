namespace QuanLySinhVien.Models
{
    public class CourseClassModel
    {
        public string CourseClassId { get; set; }
        public string CourseId { get; set; }
        public string Room { get; set; }
        public string LearnSchedule { get; set; }
        public string Duration { get; set; }
        public int Semester { get; set; }
        public int Year { get; set; }
        public string Status { get; set; }
        public int Capacity { get; set; }
    }
}