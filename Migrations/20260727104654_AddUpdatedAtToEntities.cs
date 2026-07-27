using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskFlowMvc.Migrations
{
    /// <inheritdoc />
    public partial class AddUpdatedAtToEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Workspace",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "TaskState",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "TaskItem",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Tag",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Tag",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Project",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "TaskState",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "UpdatedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "TaskState",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "UpdatedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "TaskState",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "UpdatedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "TaskState",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                column: "UpdatedAt",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Workspace");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "TaskState");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "TaskItem");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Project");
        }
    }
}
