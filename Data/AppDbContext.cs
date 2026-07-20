using Microsoft.EntityFrameworkCore;
using TeacherWebPage.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<GradeLevel> GradeLevels => Set<GradeLevel>();
    public DbSet<Group> Groups => Set<Group>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<AttendanceRecord> AttendanceRecords => Set<AttendanceRecord>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Seeding account افتراضي للمدرس عشان تعرف تسجل بيه أول مرة
        modelBuilder.Entity<User>().HasData(new User
        {
            Id = 1,
            Username = "admin",
            // كلمة السر المبدئية: "Admin123" (محمية بـ BCrypt أو SHA256)
            PasswordHash = "$2a$11$qR4A.j2.J.D/4Pj9R7lDq.3tN3G7aZ.S6mG8o1v3K4X5Y6Z7A8B9C",
            FullName = "أستاذ المادة"
        });
    }
}