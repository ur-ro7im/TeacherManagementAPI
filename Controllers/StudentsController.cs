using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeacherPlatform.DTOs;
using TeacherWebPage.DTOs;
using TeacherWebPage.Models; // تأكد من استدعاء ה-DTOs صح

namespace TeacherPlatform.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StudentsController(AppDbContext context)
        {
            _context = context;
        }

        // جلب قائمة الطلاب مع الفلاتر
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search,
            [FromQuery] string? searchBy,
            [FromQuery] int? groupId,
            [FromQuery] string? statusFilter)
        {
            var now = DateTime.UtcNow;
            var today = now.Date;

            var query = _context.Students
                .Include(s => s.Group)
                .ThenInclude(g => g.GradeLevel)
                .Include(s => s.Subscriptions)
                .Include(s => s.AttendanceRecords)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                switch (searchBy)
                {
                    case "name":
                        query = query.Where(s => s.FullName.Contains(search));
                        break;
                    case "studentPhone":
                        query = query.Where(s => s.StudentPhone.Contains(search));
                        break;
                    case "parentPhone":
                        query = query.Where(s => s.ParentPhone.Contains(search));
                        break;
                    default:
                        query = query.Where(s => s.FullName.Contains(search) ||
                                                 s.StudentPhone.Contains(search) ||
                                                 s.ParentPhone.Contains(search));
                        break;
                }
            }

            if (groupId.HasValue && groupId.Value > 0)
            {
                query = query.Where(s => s.GroupId == groupId.Value);
            }

            if (!string.IsNullOrEmpty(statusFilter))
            {
                switch (statusFilter)
                {
                    case "paidThisMonth":
                        query = query.Where(s => s.Subscriptions.Any(sub => sub.Month == now.Month && sub.Year == now.Year && sub.IsPaid));
                        break;

                    case "unpaidThisMonth":
                        query = query.Where(s => !s.Subscriptions.Any(sub => sub.Month == now.Month && sub.Year == now.Year && sub.IsPaid));
                        break;

                    case "presentToday":
                        query = query.Where(s => s.AttendanceRecords.Any(a => a.Date.Date == today && a.IsPresent));
                        break;

                    case "absentToday":
                        query = query.Where(s => s.AttendanceRecords.Any(a => a.Date.Date == today && !a.IsPresent));
                        break;
                }
            }

            var result = await query.Select(s => new StudentResponseDto
            {
                Id = s.Id,
                FullName = s.FullName,
                StudentPhone = s.StudentPhone,
                ParentPhone = s.ParentPhone,
                CreatedAt = s.CreatedAt,
                GroupId = s.GroupId,
                GroupName = s.Group.Name,
                GradeLevelName = s.Group.GradeLevel.Name
            }).ToListAsync();

            return Ok(result);
        }

        // 🔴 جلب التفاصيل الشاملة للطالب (الاشتراكات والحضور)
        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetStudentDetails(int id)
        {
            var student = await _context.Students
                .Include(s => s.Group)
                .ThenInclude(g => g.GradeLevel)
                .Include(s => s.Subscriptions)
                .Include(s => s.AttendanceRecords)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null) return NotFound("الطالب غير موجود");

            var details = new StudentDetailsDto
            {
                Id = student.Id,
                FullName = student.FullName,
                StudentPhone = student.StudentPhone,
                ParentPhone = student.ParentPhone,
                GroupName = student.Group?.Name ?? "بدون مجموعة",
                GradeLevelName = student.Group?.GradeLevel?.Name ?? "بدون صف",
                CreatedAt = student.CreatedAt,
                Subscriptions = student.Subscriptions
                    .OrderByDescending(sub => sub.Year)
                    .ThenByDescending(sub => sub.Month)
                    .Select(sub => new SubscriptionItemDto
                    {
                        Id = sub.Id,
                        Month = sub.Month,
                        Year = sub.Year,
                        Amount = sub.Amount,
                        IsPaid = sub.IsPaid,
                        PaymentDate = sub.PaymentDate
                    }).ToList(),
                AttendanceRecords = student.AttendanceRecords
                    .OrderByDescending(a => a.Date)
                    .Select(a => new AttendanceItemDto
                    {
                        Id = a.Id,
                        Date = a.Date,
                        IsPresent = a.IsPresent
                    }).ToList()
            };

            return Ok(details);
        }

        // إضافة طالب جديد
        [HttpPost]
        public async Task<IActionResult> Create(CreateStudentDto dto)
        {
            var student = new Student
            {
                FullName = dto.FullName,
                StudentPhone = dto.StudentPhone,
                ParentPhone = dto.ParentPhone,
                GroupId = dto.GroupId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Students.Add(student);
            await _context.SaveChangesAsync();
            return Ok(student);
        }

        // تعديل طالب
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CreateStudentDto dto)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null) return NotFound("الطالب غير موجود");

            student.FullName = dto.FullName;
            student.StudentPhone = dto.StudentPhone;
            student.ParentPhone = dto.ParentPhone;
            student.GroupId = dto.GroupId;

            await _context.SaveChangesAsync();
            return Ok(student);
        }

        // حذف طالب
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null) return NotFound("الطالب غير موجود");

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}