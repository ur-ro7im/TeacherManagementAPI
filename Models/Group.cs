namespace TeacherWebPage.Models
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // مثل: مجموعة السبت 4 مساءً

        public int GradeLevelId { get; set; }
        public GradeLevel GradeLevel { get; set; } = null!;

        public ICollection<Student> Students { get; set; } = new List<Student>();
    }
}
