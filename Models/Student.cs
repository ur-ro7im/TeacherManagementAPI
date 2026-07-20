namespace TeacherWebPage.Models
{
    // 4. Student (الطالب)
    public class Student
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string StudentPhone { get; set; } = string.Empty;
        public string ParentPhone { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int GroupId { get; set; }
        public Group Group { get; set; } = null!;

        public ICollection<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();
        public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
    }
}
