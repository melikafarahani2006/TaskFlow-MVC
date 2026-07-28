using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskFlowMvc.Migrations
{
    /// <inheritdoc />
    public partial class RenameTaskItemAndTaskItemTag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Project_Workspace_WorkspaceId",
                table: "Project");

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
                name: "FK_TaskItemTag_TaskItem_TaskItemId",
                table: "TaskItemTag");

            migrationBuilder.RenameColumn(
                name: "TaskItemId",
                table: "TaskItemTag",
                newName: "TaskId");

            migrationBuilder.RenameIndex(
                name: "IX_TaskItemTag_TaskItemId",
                table: "TaskItemTag",
                newName: "IX_TaskItemTag_TaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_Project_Workspace_WorkspaceId",
                table: "Project",
                column: "WorkspaceId",
                principalTable: "Workspace",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Project_Workspace_WorkspaceId",
                table: "Project");

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

            migrationBuilder.RenameColumn(
                name: "TaskId",
                table: "TaskItemTag",
                newName: "TaskItemId");

            migrationBuilder.RenameIndex(
                name: "IX_TaskItemTag_TaskId",
                table: "TaskItemTag",
                newName: "IX_TaskItemTag_TaskItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Project_Workspace_WorkspaceId",
                table: "Project",
                column: "WorkspaceId",
                principalTable: "Workspace",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskItem_Project_ProjectId",
                table: "TaskItem",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskItem_TaskState_TaskStateId",
                table: "TaskItem",
                column: "TaskStateId",
                principalTable: "TaskState",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskItemTag_Tag_TagId",
                table: "TaskItemTag",
                column: "TagId",
                principalTable: "Tag",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskItemTag_TaskItem_TaskItemId",
                table: "TaskItemTag",
                column: "TaskItemId",
                principalTable: "TaskItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
