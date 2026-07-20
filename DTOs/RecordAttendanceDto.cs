namespace TeacherWebPage.DTOs
{
    // DTOs/AttendanceDtos.cs
    public class RecordAttendanceDto
    {
        public int StudentId { get; set; }
        public DateTime Date { get; set; }
        public bool IsPresent { get; set; }
    }
}
