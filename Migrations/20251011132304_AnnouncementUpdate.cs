using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ST10296167_PROG7312_POE.Migrations
{
    /// <inheritdoc />
    public partial class AnnouncementUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Announcements");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Announcements",
                type: "TEXT",
                nullable: false,
                defaultValueSql: "datetime('now')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Announcements");

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "Announcements",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
