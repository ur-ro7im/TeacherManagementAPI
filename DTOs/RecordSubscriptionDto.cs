namespace TeacherWebPage.DTOs
{
    // DTOs/SubscriptionDtos.cs
    public class RecordSubscriptionDto
    {
        public int StudentId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal Amount { get; set; }
        public bool IsPaid { get; set; }
    }
}
