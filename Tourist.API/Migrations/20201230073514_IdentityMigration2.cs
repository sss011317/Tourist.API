using Microsoft.EntityFrameworkCore.Migrations;

namespace Tourist.API.Migrations
{
    public partial class IdentityMigration2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "308660dc-ae51-480f-824d-7dca6714c3e2",
                column: "ConcurrencyStamp",
                value: "1351468b-08ed-4b2a-aeaa-bc335a668145");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "90184155-dee0-40c9-bb1e-b5ed07afc04e",
                columns: new[] { "ConcurrencyStamp", "Email", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "SecurityStamp", "UserName" },
                values: new object[] { "d724e7de-7307-4294-8343-50e8391603af", "admin@tourist.com", "ADMIN@TOURIST.COM", "ADMIN@TOURIST.COM", "AQAAAAEAACcQAAAAELTebG/9JLN9UJz+vKc54WlIj+sBMovVz12pf6sXAgnSRh7MdgVmI1Q6NMNOT28zPw==", "98891197-4a36-411c-bf4d-0b82cbe0c854", "admin@tourist.com" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "308660dc-ae51-480f-824d-7dca6714c3e2",
                column: "ConcurrencyStamp",
                value: "3bacca66-a999-4aa9-ac09-385aad0b8f06");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "90184155-dee0-40c9-bb1e-b5ed07afc04e",
                columns: new[] { "ConcurrencyStamp", "Email", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "SecurityStamp", "UserName" },
                values: new object[] { "bac21cf2-0348-4260-b1a9-49606959fb40", "admin@fakexiecheng.com", "ADMIN@FAKEXIECHENG.COM", "ADMIN@FAKEXIECHENG.COM", "AQAAAAEAACcQAAAAEJlv6SXfn1ijqbKriqrg0mFRZDgA4JydOpnr1PEqm0pZfgYwACvlOLPCLL1UiROZHA==", "37cd3f5c-841a-4ffa-8e1e-ccf37a7b5f9d", "admin@fakexiecheng.com" });
        }
    }
}
