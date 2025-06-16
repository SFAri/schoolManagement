using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagement.Migrations
{
    public partial class Notification_Add : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ✅ Tạo bảng Notifications mà KHÔNG xóa dữ liệu
            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            // ✅ Nếu bạn muốn cập nhật lại thời gian CreatedAt cho AcademicYear có sẵn (nếu có)
            migrationBuilder.UpdateData(
                table: "AcademicYears",
                keyColumn: "AcademicYearId",
                keyValue: 2,
                column: "CreatedAt",
                value: DateTime.Now
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifications");
        }
    }
}
