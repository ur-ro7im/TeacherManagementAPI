namespace TeacherWebPage.DTOs
{
    public class GroupResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int GradeLevelId { get; set; }
        public string GradeLevelName { get; set; } = string.Empty;
    }
}
