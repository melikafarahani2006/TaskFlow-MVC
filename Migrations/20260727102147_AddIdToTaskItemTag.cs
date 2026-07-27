using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskFlowMvc.Migrations
{
    /// <inheritdoc />
    public partial class AddIdToTaskItemTag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskItemTag",
                table: "TaskItemTag");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "TaskItemTag",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskItemTag",
                table: "TaskItemTag",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_TaskItemTag_TaskItemId",
                table: "TaskItemTag",
                column: "TaskItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskItemTag",
                table: "TaskItemTag");

            migrationBuilder.DropIndex(
                name: "IX_TaskItemTag_TaskItemId",
                table: "TaskItemTag");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "TaskItemTag");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskItemTag",
                table: "TaskItemTag",
                columns: new[] { "TaskItemId", "TagId" });
        }
    }
}
