using System;

public class StudentSubmitModel
{
    public int Id { get; set; }
    public int AssignmentId { get; set; }
    public string StudentId { get; set; }
    public string SubmitContent { get; set; }
    public DateTime SubmitDate { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
}
