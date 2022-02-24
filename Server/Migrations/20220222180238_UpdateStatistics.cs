using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VueStart.Migrations
{
    public partial class UpdateStatistics : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ViewGeneratedCount",
                table: "StatisticRecords",
                newName: "WizardGeneratedCount");

            migrationBuilder.RenameColumn(
                name: "ViewDownloadedCount",
                table: "StatisticRecords",
                newName: "WizardDownloadedCount");

            migrationBuilder.RenameColumn(
                name: "FormGeneratedCount",
                table: "StatisticRecords",
                newName: "TableGeneratedCount");

            migrationBuilder.RenameColumn(
                name: "FormDownloadedCount",
                table: "StatisticRecords",
                newName: "TableDownloadedCount");

            migrationBuilder.RenameColumn(
                name: "EditorGeneratedCount",
                table: "StatisticRecords",
                newName: "CardGeneratedCount");

            migrationBuilder.RenameColumn(
                name: "EditorDownloadedCount",
                table: "StatisticRecords",
                newName: "CardDownloadedCount");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WizardGeneratedCount",
                table: "StatisticRecords",
                newName: "ViewGeneratedCount");

            migrationBuilder.RenameColumn(
                name: "WizardDownloadedCount",
                table: "StatisticRecords",
                newName: "ViewDownloadedCount");

            migrationBuilder.RenameColumn(
                name: "TableGeneratedCount",
                table: "StatisticRecords",
                newName: "FormGeneratedCount");

            migrationBuilder.RenameColumn(
                name: "TableDownloadedCount",
                table: "StatisticRecords",
                newName: "FormDownloadedCount");

            migrationBuilder.RenameColumn(
                name: "CardGeneratedCount",
                table: "StatisticRecords",
                newName: "EditorGeneratedCount");

            migrationBuilder.RenameColumn(
                name: "CardDownloadedCount",
                table: "StatisticRecords",
                newName: "EditorDownloadedCount");
        }
    }
}
