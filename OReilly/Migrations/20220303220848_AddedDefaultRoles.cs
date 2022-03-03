using Microsoft.EntityFrameworkCore.Migrations;

namespace OReilly.Migrations
{
    public partial class AddedDefaultRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "7d39c5a3-1d83-45e8-bd32-20eb7b99f24a", "b97b89e2-89e0-4f6a-aff2-0de8177dcbab", "User", "USER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "60abf779-4205-480e-b73c-a8735cda7ee7", "f27e2c22-74ac-4710-ae93-289b9efa6a77", "Administrator", "ADMINISTRATOR" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "60abf779-4205-480e-b73c-a8735cda7ee7");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7d39c5a3-1d83-45e8-bd32-20eb7b99f24a");
        }
    }
}
