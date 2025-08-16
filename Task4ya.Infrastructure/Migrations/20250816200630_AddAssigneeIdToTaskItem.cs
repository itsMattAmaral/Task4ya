using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Task4ya.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAssigneeIdToTaskItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "TaskItems",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "TaskItems",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AssigneeToId",
                table: "TaskItems",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskItems_AssigneeToId",
                table: "TaskItems",
                column: "AssigneeToId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskItems_BoardId",
                table: "TaskItems",
                column: "BoardId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskItems_Boards_BoardId",
                table: "TaskItems",
                column: "BoardId",
                principalTable: "Boards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskItems_Users_AssigneeToId",
                table: "TaskItems",
                column: "AssigneeToId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskItems_Boards_BoardId",
                table: "TaskItems");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskItems_Users_AssigneeToId",
                table: "TaskItems");

            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_TaskItems_AssigneeToId",
                table: "TaskItems");

            migrationBuilder.DropIndex(
                name: "IX_TaskItems_BoardId",
                table: "TaskItems");

            migrationBuilder.DropColumn(
                name: "AssigneeToId",
                table: "TaskItems");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "TaskItems",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "TaskItems",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000,
                oldNullable: true);
        }
    }
}
