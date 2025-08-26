using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Task4ya.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOwnerIdToBoard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Boards",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<int>(
                name: "OwnerId",
                table: "Boards",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Boards",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Boards");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Boards");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Boards");
        }
    }
}
