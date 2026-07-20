using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeacherWebPage.DTOs;
using TeacherWebPage.Models;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class GroupsController : ControllerBase
{
    private readonly AppDbContext _context;
    public GroupsController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var groups = await _context.Groups
            .Include(g => g.GradeLevel)
            .Select(g => new GroupResponseDto
            {
                Id = g.Id,
                Name = g.Name,
                GradeLevelId = g.GradeLevelId,
                GradeLevelName = g.GradeLevel.Name
            }).ToListAsync();

        return Ok(groups);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateGroupDto dto)
    {
        var group = new Group { Name = dto.Name, GradeLevelId = dto.GradeLevelId };
        _context.Groups.Add(group);
        await _context.SaveChangesAsync();
        return Ok(group);
    }

    // 🔴 تعديل مجموعة
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CreateGroupDto dto)
    {
        var group = await _context.Groups.FindAsync(id);
        if (group == null) return NotFound("المجموعة غير موجودة");

        group.Name = dto.Name;
        group.GradeLevelId = dto.GradeLevelId;

        await _context.SaveChangesAsync();
        return Ok(group);
    }

    // 🔴 حذف مجموعة
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var group = await _context.Groups
            .Include(g => g.Students)
            .FirstOrDefaultAsync(g => g.Id == id);

        if (group == null) return NotFound("المجموعة غير موجودة");

        // تنبيه في حالة وجود طلاب مرتبطين بالمجموعة
        if (group.Students.Any())
        {
            return BadRequest("لا يمكن حذف المجموعة لأن بها طلاب. قم بنقل الطلاب أولاً أو حذفهم.");
        }

        _context.Groups.Remove(group);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}