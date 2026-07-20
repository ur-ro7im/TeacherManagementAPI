namespace TeacherWebPage.DTOs
{
    // DTOs/StudentDtos.cs
    public class CreateStudentDto
    {
        public string FullName { get; set; } = string.Empty;
        public string StudentPhone { get; set; } = string.Empty;
        public string ParentPhone { get; set; } = string.Empty;
        public int GroupId { get; set; }
    }
}
