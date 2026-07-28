using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskFlowMvc.Migrations
{
    /// <inheritdoc />
    public partial class RenameTaskItemAndTaskItemTagEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskItem_Project_ProjectId",
                table: "TaskItem");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskItem_TaskState_TaskStateId",
                table: "TaskItem");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskItemTag_Tag_TagId",
                table: "TaskItemTag");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskItemTag_TaskItem_TaskId",
                table: "TaskItemTag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskItemTag",
                table: "TaskItemTag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskItem",
                table: "TaskItem");

            migrationBuilder.RenameTable(
                name: "TaskItemTag",
                newName: "TaskTag");

            migrationBuilder.RenameTable(
                name: "TaskItem",
                newName: "Task");

            migrationBuilder.RenameIndex(
                name: "IX_TaskItemTag_TaskId",
                table: "TaskTag",
                newName: "IX_TaskTag_TaskId");

            migrationBuilder.RenameIndex(
                name: "IX_TaskItemTag_TagId",
                table: "TaskTag",
                newName: "IX_TaskTag_TagId");

            migrationBuilder.RenameIndex(
                name: "IX_TaskItem_TaskStateId",
                table: "Task",
                newName: "IX_Task_TaskStateId");

            migrationBuilder.RenameIndex(
                name: "IX_TaskItem_ProjectId",
                table: "Task",
                newName: "IX_Task_ProjectId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskTag",
                table: "TaskTag",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Task",
                table: "Task",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Task_Project_ProjectId",
                table: "Task",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Task_TaskState_TaskStateId",
                table: "Task",
                column: "TaskStateId",
                principalTable: "TaskState",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskTag_Tag_TagId",
                table: "TaskTag",
                column: "TagId",
                principalTable: "Tag",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskTag_Task_TaskId",
                table: "TaskTag",
                column: "TaskId",
                principalTable: "Task",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Task_Project_ProjectId",
                table: "Task");

            migrationBuilder.DropForeignKey(
                name: "FK_Task_TaskState_TaskStateId",
                table: "Task");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskTag_Tag_TagId",
                table: "TaskTag");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskTag_Task_TaskId",
                table: "TaskTag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskTag",
                table: "TaskTag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Task",
                table: "Task");

            migrationBuilder.RenameTable(
                name: "TaskTag",
                newName: "TaskItemTag");

            migrationBuilder.RenameTable(
                name: "Task",
                newName: "TaskItem");

            migrationBuilder.RenameIndex(
                name: "IX_TaskTag_TaskId",
                table: "TaskItemTag",
                newName: "IX_TaskItemTag_TaskId");

            migrationBuilder.RenameIndex(
                name: "IX_TaskTag_TagId",
                table: "TaskItemTag",
                newName: "IX_TaskItemTag_TagId");

            migrationBuilder.RenameIndex(
                name: "IX_Task_TaskStateId",
                table: "TaskItem",
                newName: "IX_TaskItem_TaskStateId");

            migrationBuilder.RenameIndex(
                name: "IX_Task_ProjectId",
                table: "TaskItem",
                newName: "IX_TaskItem_ProjectId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskItemTag",
                table: "TaskItemTag",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskItem",
                table: "TaskItem",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskItem_Project_ProjectId",
                table: "TaskItem",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskItem_TaskState_TaskStateId",
                table: "TaskItem",
                column: "TaskStateId",
                principalTable: "TaskState",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskItemTag_Tag_TagId",
                table: "TaskItemTag",
                column: "TagId",
                principalTable: "Tag",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskItemTag_TaskItem_TaskId",
                table: "TaskItemTag",
                column: "TaskId",
                principalTable: "TaskItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
