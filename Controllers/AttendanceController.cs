using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeacherWebPage.Models; // 👈 استدعاء الموديلات صح

namespace TeacherWebPage.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AttendanceController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AttendanceController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/attendance?date=2026-07-20&groupId=1
        [HttpGet]
        public async Task<IActionResult> GetAttendance(
            [FromQuery] DateTime? date,
            [FromQuery] int? groupId)
        {
            var targetDate = (date ?? DateTime.UtcNow).Date;

            var query = _context.Students
                .Include(s => s.Group)
                .ThenInclude(g => g.GradeLevel)
                .Include(s => s.AttendanceRecords)
                .AsQueryable();

            if (groupId.HasValue && groupId.Value > 0)
            {
                query = query.Where(s => s.GroupId == groupId.Value);
            }

            var result = await query.Select(s => new
            {
                StudentId = s.Id,
                FullName = s.FullName,
                StudentPhone = s.StudentPhone,
                GroupName = s.Group.Name,
                GradeLevelName = s.Group.GradeLevel.Name,
                GroupId = s.GroupId,
                AttendanceStatus = s.AttendanceRecords
                    .Where(a => a.Date.Date == targetDate)
                    .Select(a => new
                    {
                        a.Id,
                        a.IsPresent
                    })
                    .FirstOrDefault()
            }).ToListAsync();

            return Ok(result);
        }

        // POST: api/attendance
        [HttpPost]
        public async Task<IActionResult> RecordAttendance([FromBody] RecordAttendanceDto dto)
        {
            var targetDate = dto.Date.Date;

            var existingRecord = await _context.AttendanceRecords
                .FirstOrDefaultAsync(a => a.StudentId == dto.StudentId && a.Date.Date == targetDate);

            if (existingRecord != null)
            {
                existingRecord.IsPresent = dto.IsPresent;
            }
            else
            {
                var newRecord = new AttendanceRecord
                {
                    StudentId = dto.StudentId,
                    Date = targetDate,
                    IsPresent = dto.IsPresent
                };

                _context.AttendanceRecords.Add(newRecord);
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "تم تسجيل الحضور بنجاح" });
        }

        // POST: api/attendance/bulk
        [HttpPost("bulk")]
        public async Task<IActionResult> RecordBulkAttendance([FromBody] BulkAttendanceDto dto)
        {
            var targetDate = dto.Date.Date;

            var studentIds = await _context.Students
                .Where(s => s.GroupId == dto.GroupId)
                .Select(s => s.Id)
                .ToListAsync();

            foreach (var studentId in studentIds)
            {
                var existingRecord = await _context.AttendanceRecords
                    .FirstOrDefaultAsync(a => a.StudentId == studentId && a.Date.Date == targetDate);

                if (existingRecord != null)
                {
                    existingRecord.IsPresent = dto.IsPresent;
                }
                else
                {
                    var newRecord = new AttendanceRecord
                    {
                        StudentId = studentId,
                        Date = targetDate,
                        IsPresent = dto.IsPresent
                    };

                    _context.AttendanceRecords.Add(newRecord);
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "تم تحديث المجموعة بالكامل بنجاح" });
        }
    }

    public class RecordAttendanceDto
    {
        public int StudentId { get; set; }
        public DateTime Date { get; set; }
        public bool IsPresent { get; set; }
    }

    public class BulkAttendanceDto
    {
        public int GroupId { get; set; }
        public DateTime Date { get; set; }
        public bool IsPresent { get; set; }
    }
}