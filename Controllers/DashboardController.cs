using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly AppDbContext _context;
    public DashboardController(AppDbContext context) => _context = context;

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var now = DateTime.UtcNow;
        var today = now.Date;

        var totalStudents = await _context.Students.CountAsync();

        // إحصائيات الاشتراكات للشهر الحالي
        var paidThisMonth = await _context.Subscriptions
            .CountAsync(s => s.Month == now.Month && s.Year == now.Year && s.IsPaid);

        var unpaidThisMonth = totalStudents - paidThisMonth;

        // إحصائيات الحضور والغياب لليوم
        var presentToday = await _context.AttendanceRecords
            .CountAsync(a => a.Date.Date == today && a.IsPresent);

        var absentToday = await _context.AttendanceRecords
            .CountAsync(a => a.Date.Date == today && !a.IsPresent);

        return Ok(new
        {
            TotalStudents = totalStudents,
            PaidThisMonth = paidThisMonth,
            UnpaidThisMonth = unpaidThisMonth < 0 ? 0 : unpaidThisMonth,
            PresentToday = presentToday,
            AbsentToday = absentToday
        });
    }
}