namespace TeacherPlatform.DTOs
{
    public class StudentDetailsDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string StudentPhone { get; set; } = string.Empty;
        public string ParentPhone { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;
        public string GradeLevelName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public List<SubscriptionItemDto> Subscriptions { get; set; } = new();
        public List<AttendanceItemDto> AttendanceRecords { get; set; } = new();
    }

    public class SubscriptionItemDto
    {
        public int Id { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal Amount { get; set; }
        public bool IsPaid { get; set; }
        public DateTime? PaymentDate { get; set; }
    }

    public class AttendanceItemDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public bool IsPresent { get; set; }
    }
}