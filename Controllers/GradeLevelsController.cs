using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeacherPlatform.DTOs;
using TeacherWebPage.Models;

namespace TeacherPlatform.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class GradeLevelsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GradeLevelsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/gradelevels
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var grades = await _context.GradeLevels
                    .AsNoTracking()
                    .Select(g => new GradeLevelResponseDto
                    {
                        Id = g.Id,
                        Name = g.Name ?? string.Empty,
                        MonthlyFee = g.MonthlyFee
                    })
                    .ToListAsync();

                return Ok(grades);
            }
            catch (Exception ex)
            {
                // طباعة الاستثناء في الـ Console لمعرفة السبب إن وجد
                Console.WriteLine($"Error fetching grade levels: {ex.Message}");
                return StatusCode(500, "حدث خطأ في السيرفر أثناء جلب الصفوف الدراسية");
            }
        }

        // POST: api/gradelevels
        [HttpPost]
        public async Task<IActionResult> Create(CreateGradeLevelDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Name))
            {
                return BadRequest("اسم الصف الدراسي مطلوب");
            }

            var grade = new GradeLevel
            {
                Name = dto.Name,
                MonthlyFee = dto.MonthlyFee
            };

            _context.GradeLevels.Add(grade);
            await _context.SaveChangesAsync();

            return Ok(new GradeLevelResponseDto
            {
                Id = grade.Id,
                Name = grade.Name,
                MonthlyFee = grade.MonthlyFee
            });
        }

        // PUT: api/gradelevels/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CreateGradeLevelDto dto)
        {
            var grade = await _context.GradeLevels.FindAsync(id);
            if (grade == null) return NotFound("الصف غير موجود");

            grade.Name = dto.Name;
            grade.MonthlyFee = dto.MonthlyFee;

            await _context.SaveChangesAsync();

            return Ok(new GradeLevelResponseDto
            {
                Id = grade.Id,
                Name = grade.Name,
                MonthlyFee = grade.MonthlyFee
            });
        }

        // DELETE: api/gradelevels/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var grade = await _context.GradeLevels
                .Include(g => g.Groups)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (grade == null) return NotFound("الصف غير موجود");

            if (grade.Groups != null && grade.Groups.Any())
            {
                return BadRequest("لا يمكن حذف الصف الدراسي لأنه يحتوي على مجموعات نشطة.");
            }

            _context.GradeLevels.Remove(grade);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}