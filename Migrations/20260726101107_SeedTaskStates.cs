using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TaskFlowMvc.Migrations
{
    /// <inheritdoc />
    public partial class SeedTaskStates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskState_Project_ProjectId",
                table: "TaskState");

            migrationBuilder.DropIndex(
                name: "IX_TaskState_ProjectId",
                table: "TaskState");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "TaskState");

            migrationBuilder.RenameColumn(
                name: "TaskListId",
                table: "TaskItem",
                newName: "ProjectId");

            migrationBuilder.InsertData(
                table: "TaskState",
                columns: new[] { "Id", "CreatedAt", "Name", "Order" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Todo", 0 },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "In Progress", 0 },
                    { new Guid("33333333-3333-3333-3333-333333333333"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Review", 0 },
                    { new Guid("44444444-4444-4444-4444-444444444444"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Done", 0 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskItem_ProjectId",
                table: "TaskItem",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskItem_Project_ProjectId",
                table: "TaskItem",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskItem_Project_ProjectId",
                table: "TaskItem");

            migrationBuilder.DropIndex(
                name: "IX_TaskItem_ProjectId",
                table: "TaskItem");

            migrationBuilder.DeleteData(
                table: "TaskState",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "TaskState",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "TaskState",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "TaskState",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"));

            migrationBuilder.RenameColumn(
                name: "ProjectId",
                table: "TaskItem",
                newName: "TaskListId");

            migrationBuilder.AddColumn<Guid>(
                name: "ProjectId",
                table: "TaskState",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_TaskState_ProjectId",
                table: "TaskState",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskState_Project_ProjectId",
                table: "TaskState",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
