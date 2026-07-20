namespace TeacherWebPage.DTOs
{
    public class StudentResponseDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string StudentPhone { get; set; } = string.Empty;
        public string ParentPhone { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int GroupId { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public string GradeLevelName { get; set; } = string.Empty;
    }
}
