using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagement.Migrations
{
    public partial class SeedRoles_Modified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6d92e936-04fa-438e-a2f6-12e78853a92a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b1adef75-e62a-4472-a309-7c6ef749f1cb");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b39f159e-ada0-4db2-92bf-a609a3401237");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "f9cb11aa-afab-47d7-b851-e00c4b0b453a");

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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "6d92e936-04fa-438e-a2f6-12e78853a92a", null, "Student", "STUDENT" },
                    { "b1adef75-e62a-4472-a309-7c6ef749f1cb", null, "Lecturer", "LECTURER" },
                    { "b39f159e-ada0-4db2-92bf-a609a3401237", null, "Admin", "ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "DOB", "Email", "EmailConfirmed", "FirstName", "Gender", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "RoleId", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "f9cb11aa-afab-47d7-b851-e00c4b0b453a", 0, "b0d1fc00-c00e-4a60-99cf-d8b19ae9ee18", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@gmail.com", false, "Katz", 0, "Ariz", false, null, "ADMIN@GMAIL.COM", "ADMIN@GMAIL.COM", "AQAAAAIAAYagAAAAEDRgMzjSfFUJMRRLzZJIq7KZyk2Hd7ikJuomyiTZjYDZC8kTwSIZghOehYj2m5xr6w==", null, false, 0, "5c532854-0ce4-43a7-a890-8e6851ac0ae8", false, "admindmin@gmail.com" });
        }
    }
}
