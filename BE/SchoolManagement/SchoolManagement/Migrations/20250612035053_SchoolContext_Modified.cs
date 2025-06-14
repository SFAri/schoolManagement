using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagement.Migrations
{
    public partial class SchoolContext_Modified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bd60cbd8-bec8-4e05-9611-e9b7be52ddbd");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d81c53ae-d756-4100-b0be-90699a1a9692");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "aafe515f-9493-4d58-9b8b-46bc9e03850a", "133fd73e-8768-42e7-8f08-ccf92302cfb4" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "aafe515f-9493-4d58-9b8b-46bc9e03850a");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "133fd73e-8768-42e7-8f08-ccf92302cfb4");

            migrationBuilder.AddColumn<int>(
                name: "AcademicYearId",
                table: "Courses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "AcademicYears",
                columns: table => new
                {
                    AcademicYearId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Year = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademicYears", x => x.AcademicYearId);
                });

            migrationBuilder.InsertData(
                table: "AcademicYears",
                columns: new[] { "AcademicYearId", "CreatedAt", "IsLocked", "Year" },
                values: new object[,]
                {
                    { 1, new DateTime(2023, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "2023-2024" },
                    { 2, new DateTime(2025, 6, 12, 10, 50, 53, 631, DateTimeKind.Local).AddTicks(8700), false, "2024-2025" }
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0851e4a9-c40e-4a53-b8b0-596550da27c8", null, "Lecturer", "LECTURER" },
                    { "4e0b76bd-4493-4ba4-ae61-47a8a608542b", null, "Admin", "ADMIN" },
                    { "cbbb038c-ede0-470f-a651-02671f4f0c98", null, "Student", "STUDENT" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "DOB", "Email", "EmailConfirmed", "FirstName", "Gender", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "RoleId", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "13f6374a-dfee-4c77-9bfa-918edfb16a3c", 0, "5c2c726e-4c2f-4153-a3ab-eac262e5acd5", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@gmail.com", false, "Katz", 0, "Ariz", false, null, "ADMIN@GMAIL.COM", "ADMIN@GMAIL.COM", "AQAAAAIAAYagAAAAEMGOkUv3mXM9YrTSH1XlWZveLUJlpWueXo4rC+wYCURZ4ZrTs+3vyiq+s+8Oo6vwqA==", null, false, 0, "673fe6ec-ceed-4038-a309-7b9987cfb197", false, "admin@gmail.com" },
                    { "74e9a246-5f13-491e-a0d2-d5fbaddf91c8", 0, "6d9842dc-3c34-4a4c-831d-e387311820fa", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "lecturer1@gmail.com", false, "Liam", 1, "Nguyen", false, null, "LECTURER1@GMAIL.COM", "LECTURER1@GMAIL.COM", "AQAAAAIAAYagAAAAEHIgbH72vgIa3xJkYDj/aD+qlw/H7l13KfuSpG71yE2+KhQdVkWYqXLjOVJJlV5ZKg==", null, false, 1, "64950a27-52d2-4e6d-ab76-48d33b795971", false, "lecturer1@gmail.com" },
                    { "a89c11b0-2099-43f3-8060-74237b6ed5cb", 0, "05bcaf33-539a-455b-92c6-51c9093ea159", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "student2@gmail.com", false, "Minh", 1, "Tran", false, null, "STUDENT2@GMAIL.COM", "STUDENT2@GMAIL.COM", "AQAAAAIAAYagAAAAEMPfiUzVaoe8zPwwX9pF6WIos1EIOyb5EToHF1T60wC7XH5UBJS3tsCSGGMXuMYswQ==", null, false, 2, "b4244218-db73-4f9d-b719-5a03a1d11c1e", false, "student2@gmail.com" },
                    { "bcfa7ac0-d384-45fe-9d2f-18761f1b012d", 0, "1281e76d-1544-4483-b6c1-a0d0dae51e64", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "student1@gmail.com", false, "Anna", 0, "Le", false, null, "STUDENT1@GMAIL.COM", "STUDENT1@GMAIL.COM", "AQAAAAIAAYagAAAAEMHPxqbyQ1HTM0KmCxbofxFkbgNJBihDKIuBpIAJXri8Xzi3YfvUWfOYU/Jq7QK8lA==", null, false, 2, "e93e4a4d-f7e6-436a-be32-d7e017effb20", false, "student1@gmail.com" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "4e0b76bd-4493-4ba4-ae61-47a8a608542b", "13f6374a-dfee-4c77-9bfa-918edfb16a3c" },
                    { "0851e4a9-c40e-4a53-b8b0-596550da27c8", "74e9a246-5f13-491e-a0d2-d5fbaddf91c8" },
                    { "cbbb038c-ede0-470f-a651-02671f4f0c98", "a89c11b0-2099-43f3-8060-74237b6ed5cb" },
                    { "cbbb038c-ede0-470f-a651-02671f4f0c98", "bcfa7ac0-d384-45fe-9d2f-18761f1b012d" }
                });

            migrationBuilder.InsertData(
                table: "Courses",
                columns: new[] { "CourseId", "AcademicYearId", "CourseName", "EndDate", "LecturerId", "StartDate", "UserId" },
                values: new object[,]
                {
                    { 1, 1, "React", new DateTime(2024, 1, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "74e9a246-5f13-491e-a0d2-d5fbaddf91c8", new DateTime(2023, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null },
                    { 2, 1, "Flutter", new DateTime(2024, 1, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "74e9a246-5f13-491e-a0d2-d5fbaddf91c8", new DateTime(2023, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null }
                });

            migrationBuilder.InsertData(
                table: "Scores",
                columns: new[] { "CourseId", "UserId", "AverageScore", "CreatedAt", "Final", "Grade", "LastUpdatedAt", "Midterm", "Process1", "Process2" },
                values: new object[,]
                {
                    { 1, "a89c11b0-2099-43f3-8060-74237b6ed5cb", 7.4f, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 7f, 2, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 10f, 7f, 6f },
                    { 2, "a89c11b0-2099-43f3-8060-74237b6ed5cb", 7.4f, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 8f, 2, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 7f, 6f, 7f },
                    { 1, "bcfa7ac0-d384-45fe-9d2f-18761f1b012d", 8.65f, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 8f, 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 10f, 8.5f, 9f },
                    { 2, "bcfa7ac0-d384-45fe-9d2f-18761f1b012d", 8.65f, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 8f, 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 10f, 8.5f, 9f }
                });

            migrationBuilder.InsertData(
                table: "Shifts",
                columns: new[] { "ShiftId", "CourseId", "MaxQuantity", "ShiftCode", "WeekDay" },
                values: new object[,]
                {
                    { 1, 1, 30, 0, 0 },
                    { 2, 1, 30, 1, 3 },
                    { 3, 2, 30, 0, 2 },
                    { 4, 2, 30, 1, 4 }
                });

            migrationBuilder.InsertData(
                table: "Enrollments",
                columns: new[] { "ShiftId", "UserId", "TimeJoined" },
                values: new object[,]
                {
                    { 1, "a89c11b0-2099-43f3-8060-74237b6ed5cb", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, "a89c11b0-2099-43f3-8060-74237b6ed5cb", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, "a89c11b0-2099-43f3-8060-74237b6ed5cb", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, "a89c11b0-2099-43f3-8060-74237b6ed5cb", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 1, "bcfa7ac0-d384-45fe-9d2f-18761f1b012d", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, "bcfa7ac0-d384-45fe-9d2f-18761f1b012d", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, "bcfa7ac0-d384-45fe-9d2f-18761f1b012d", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, "bcfa7ac0-d384-45fe-9d2f-18761f1b012d", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Courses_AcademicYearId",
                table: "Courses",
                column: "AcademicYearId");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_AcademicYears_AcademicYearId",
                table: "Courses",
                column: "AcademicYearId",
                principalTable: "AcademicYears",
                principalColumn: "AcademicYearId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_AcademicYears_AcademicYearId",
                table: "Courses");

            migrationBuilder.DropTable(
                name: "AcademicYears");

            migrationBuilder.DropIndex(
                name: "IX_Courses_AcademicYearId",
                table: "Courses");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "4e0b76bd-4493-4ba4-ae61-47a8a608542b", "13f6374a-dfee-4c77-9bfa-918edfb16a3c" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "0851e4a9-c40e-4a53-b8b0-596550da27c8", "74e9a246-5f13-491e-a0d2-d5fbaddf91c8" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "cbbb038c-ede0-470f-a651-02671f4f0c98", "a89c11b0-2099-43f3-8060-74237b6ed5cb" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "cbbb038c-ede0-470f-a651-02671f4f0c98", "bcfa7ac0-d384-45fe-9d2f-18761f1b012d" });

            migrationBuilder.DeleteData(
                table: "Enrollments",
                keyColumns: new[] { "ShiftId", "UserId" },
                keyValues: new object[] { 1, "a89c11b0-2099-43f3-8060-74237b6ed5cb" });

            migrationBuilder.DeleteData(
                table: "Enrollments",
                keyColumns: new[] { "ShiftId", "UserId" },
                keyValues: new object[] { 2, "a89c11b0-2099-43f3-8060-74237b6ed5cb" });

            migrationBuilder.DeleteData(
                table: "Enrollments",
                keyColumns: new[] { "ShiftId", "UserId" },
                keyValues: new object[] { 3, "a89c11b0-2099-43f3-8060-74237b6ed5cb" });

            migrationBuilder.DeleteData(
                table: "Enrollments",
                keyColumns: new[] { "ShiftId", "UserId" },
                keyValues: new object[] { 4, "a89c11b0-2099-43f3-8060-74237b6ed5cb" });

            migrationBuilder.DeleteData(
                table: "Enrollments",
                keyColumns: new[] { "ShiftId", "UserId" },
                keyValues: new object[] { 1, "bcfa7ac0-d384-45fe-9d2f-18761f1b012d" });

            migrationBuilder.DeleteData(
                table: "Enrollments",
                keyColumns: new[] { "ShiftId", "UserId" },
                keyValues: new object[] { 2, "bcfa7ac0-d384-45fe-9d2f-18761f1b012d" });

            migrationBuilder.DeleteData(
                table: "Enrollments",
                keyColumns: new[] { "ShiftId", "UserId" },
                keyValues: new object[] { 3, "bcfa7ac0-d384-45fe-9d2f-18761f1b012d" });

            migrationBuilder.DeleteData(
                table: "Enrollments",
                keyColumns: new[] { "ShiftId", "UserId" },
                keyValues: new object[] { 4, "bcfa7ac0-d384-45fe-9d2f-18761f1b012d" });

            migrationBuilder.DeleteData(
                table: "Scores",
                keyColumns: new[] { "CourseId", "UserId" },
                keyValues: new object[] { 1, "a89c11b0-2099-43f3-8060-74237b6ed5cb" });

            migrationBuilder.DeleteData(
                table: "Scores",
                keyColumns: new[] { "CourseId", "UserId" },
                keyValues: new object[] { 2, "a89c11b0-2099-43f3-8060-74237b6ed5cb" });

            migrationBuilder.DeleteData(
                table: "Scores",
                keyColumns: new[] { "CourseId", "UserId" },
                keyValues: new object[] { 1, "bcfa7ac0-d384-45fe-9d2f-18761f1b012d" });

            migrationBuilder.DeleteData(
                table: "Scores",
                keyColumns: new[] { "CourseId", "UserId" },
                keyValues: new object[] { 2, "bcfa7ac0-d384-45fe-9d2f-18761f1b012d" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0851e4a9-c40e-4a53-b8b0-596550da27c8");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4e0b76bd-4493-4ba4-ae61-47a8a608542b");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cbbb038c-ede0-470f-a651-02671f4f0c98");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "13f6374a-dfee-4c77-9bfa-918edfb16a3c");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a89c11b0-2099-43f3-8060-74237b6ed5cb");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "bcfa7ac0-d384-45fe-9d2f-18761f1b012d");

            migrationBuilder.DeleteData(
                table: "Shifts",
                keyColumn: "ShiftId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Shifts",
                keyColumn: "ShiftId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Shifts",
                keyColumn: "ShiftId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Shifts",
                keyColumn: "ShiftId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "CourseId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "CourseId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "74e9a246-5f13-491e-a0d2-d5fbaddf91c8");

            migrationBuilder.DropColumn(
                name: "AcademicYearId",
                table: "Courses");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "aafe515f-9493-4d58-9b8b-46bc9e03850a", null, "Admin", "ADMIN" },
                    { "bd60cbd8-bec8-4e05-9611-e9b7be52ddbd", null, "Lecturer", "LECTURER" },
                    { "d81c53ae-d756-4100-b0be-90699a1a9692", null, "Student", "STUDENT" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "DOB", "Email", "EmailConfirmed", "FirstName", "Gender", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "RoleId", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "133fd73e-8768-42e7-8f08-ccf92302cfb4", 0, "842d9a81-2d41-42f5-b36d-7823a0916a9b", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@gmail.com", false, "Katz", 0, "Ariz", false, null, "ADMIN@GMAIL.COM", "ADMIN@GMAIL.COM", "AQAAAAIAAYagAAAAEDfjBkLeDHxgUPw2LREvgQATZhoLWQ3YYkpVTSBhvYYVQgwao/VE/dyrT2SkBVJa0g==", null, false, 0, "83508a40-49fd-4604-bd7f-7e19f53cabb9", false, "admin@gmail.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "aafe515f-9493-4d58-9b8b-46bc9e03850a", "133fd73e-8768-42e7-8f08-ccf92302cfb4" });
        }
    }
}
