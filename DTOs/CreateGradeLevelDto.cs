namespace TeacherPlatform.DTOs
{
    public class CreateGradeLevelDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal MonthlyFee { get; set; } // 👈 استقبال السعر
    }

    public class GradeLevelResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal MonthlyFee { get; set; }
    }
}