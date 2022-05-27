using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VueStart.Migrations
{
    public partial class ShareableLinkSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "ShareableLinks");

            migrationBuilder.DropColumn(
                name: "Editable",
                table: "ShareableLinks");

            migrationBuilder.DropColumn(
                name: "FrontendType",
                table: "ShareableLinks");

            migrationBuilder.RenameColumn(
                name: "Json",
                table: "ShareableLinks",
                newName: "GenerateRequest");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEIyo9Z9EEDv7Dmyv5tqUNQ6pgWN6HV2kzycBoFanFRBjNxClWbdgQe2BSCq5ldV4Mw==");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GenerateRequest",
                table: "ShareableLinks",
                newName: "Json");

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "ShareableLinks",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Editable",
                table: "ShareableLinks",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "FrontendType",
                table: "ShareableLinks",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEP2ySbRDxPLVAzU13JfKBPpxhk2Vjh0LzEe29VP+MuGnKdzeD8BuGR5Dd1oo7gFzzA==");
        }
    }
}
