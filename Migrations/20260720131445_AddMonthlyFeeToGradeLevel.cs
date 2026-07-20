using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeacherWebPage.Migrations
{
    /// <inheritdoc />
    public partial class AddMonthlyFeeToGradeLevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MonthlyFee",
                table: "GradeLevels",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MonthlyFee",
                table: "GradeLevels");
        }
    }
}
