namespace TeacherWebPage.Models
{
    // 6. Subscription (الاشتراكات)
    public class Subscription
    {
        public int Id { get; set; }
        public int Month { get; set; } // 1 to 12
        public int Year { get; set; }
        public decimal Amount { get; set; }
        public bool IsPaid { get; set; }
        public DateTime? PaymentDate { get; set; }

        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;
    }
}
