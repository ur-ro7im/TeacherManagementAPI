using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeacherWebPage.Models;

namespace TeacherPlatform.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SubscriptionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SubscriptionsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/subscriptions (جلب الاشتراكات مع التصفية)
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int? month,
            [FromQuery] int? year,
            [FromQuery] int? groupId)
        {
            var now = DateTime.UtcNow;
            int currentMonth = month ?? now.Month;
            int currentYear = year ?? now.Year;

            var query = _context.Students
                .Include(s => s.Group)
                .ThenInclude(g => g.GradeLevel)
                .Include(s => s.Subscriptions)
                .AsQueryable();

            if (groupId.HasValue && groupId.Value > 0)
            {
                query = query.Where(s => s.GroupId == groupId.Value);
            }

            var studentsWithSubscriptions = await query.Select(s => new
            {
                StudentId = s.Id,
                StudentName = s.FullName,
                StudentPhone = s.StudentPhone,
                ParentPhone = s.ParentPhone,
                GroupName = s.Group.Name,
                GradeLevelName = s.Group.GradeLevel.Name,
                MonthlyFee = s.Group.GradeLevel.MonthlyFee,
                Subscription = s.Subscriptions
                    .Where(sub => sub.Month == currentMonth && sub.Year == currentYear)
                    .Select(sub => new
                    {
                        sub.Id,
                        sub.Month,
                        sub.Year,
                        sub.Amount,
                        sub.IsPaid,
                        sub.PaymentDate
                    })
                    .FirstOrDefault()
            }).ToListAsync();

            return Ok(studentsWithSubscriptions);
        }

        // POST: api/subscriptions (تسجيل/تحديث الدفع)
        [HttpPost]
        public async Task<IActionResult> ToggleSubscription([FromBody] PaySubscriptionDto dto)
        {
            var student = await _context.Students
                .Include(s => s.Group)
                .ThenInclude(g => g.GradeLevel)
                .FirstOrDefaultAsync(s => s.Id == dto.StudentId);

            if (student == null) return NotFound("الطالب غير موجود");

            decimal amountToPay = student.Group?.GradeLevel?.MonthlyFee ?? 0;

            var existingSub = await _context.Subscriptions
                .FirstOrDefaultAsync(s => s.StudentId == dto.StudentId && s.Month == dto.Month && s.Year == dto.Year);

            if (existingSub != null)
            {
                existingSub.IsPaid = dto.IsPaid;
                existingSub.PaymentDate = dto.IsPaid ? DateTime.UtcNow : null;
                existingSub.Amount = amountToPay;
            }
            else
            {
                var newSub = new Subscription
                {
                    StudentId = dto.StudentId,
                    Month = dto.Month,
                    Year = dto.Year,
                    Amount = amountToPay,
                    IsPaid = dto.IsPaid,
                    PaymentDate = dto.IsPaid ? DateTime.UtcNow : null
                };
                _context.Subscriptions.Add(newSub);
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "تم تحديث حالة الاشتراك بنجاح" });
        }

        // POST: api/subscriptions/pay (مسار إضافي لتأمين الاتصال)
        [HttpPost("pay")]
        public async Task<IActionResult> PaySubscription([FromBody] PaySubscriptionDto dto)
        {
            return await ToggleSubscription(dto);
        }
    }

    public class PaySubscriptionDto
    {
        public int StudentId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public bool IsPaid { get; set; } = true;
    }
}