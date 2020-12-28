using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tourist.API.Migrations
{
    public partial class DataSeeding : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "TouristRoutes",
                columns: new[] { "Id", "CreateTime", "DepartureTime", "Description", "DiscountPresent", "Features", "Fees", "Notes", "OriginalPrice", "Title", "UpdateTime" },
                values: new object[] { new Guid("1953b1f1-72ef-49fa-b63f-6fd228dcbe5a"), new DateTime(2020, 12, 28, 7, 42, 34, 245, DateTimeKind.Utc).AddTicks(1748), null, "Description", null, null, null, null, 0m, "Test", null });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TouristRoutes",
                keyColumn: "Id",
                keyValue: new Guid("1953b1f1-72ef-49fa-b63f-6fd228dcbe5a"));
        }
    }
}
