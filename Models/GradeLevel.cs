using TeacherWebPage.Models;

namespace TeacherWebPage.Models
{
    public class GradeLevel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal MonthlyFee { get; set; } = 0; // 👈 سعر الشهر لهذا الصف

        public ICollection<Group> Groups { get; set; } = new List<Group>();
    }
}
