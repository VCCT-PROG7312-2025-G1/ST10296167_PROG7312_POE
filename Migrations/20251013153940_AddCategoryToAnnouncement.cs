using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ST10296167_PROG7312_POE.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryToAnnouncement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Announcements",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Announcements");
        }
    }
}
