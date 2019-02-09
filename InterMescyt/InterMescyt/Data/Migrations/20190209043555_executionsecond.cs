using Microsoft.EntityFrameworkCore.Migrations;

namespace InterMescyt.Data.Migrations
{
    public partial class executionsecond : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExecutionLines_Executions_ExecutionId",
                table: "ExecutionLines");

            migrationBuilder.AlterColumn<int>(
                name: "ExecutionId",
                table: "ExecutionLines",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Suscess",
                table: "ExecutionLines",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_ExecutionLines_Executions_ExecutionId",
                table: "ExecutionLines",
                column: "ExecutionId",
                principalTable: "Executions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExecutionLines_Executions_ExecutionId",
                table: "ExecutionLines");

            migrationBuilder.DropColumn(
                name: "Suscess",
                table: "ExecutionLines");

            migrationBuilder.AlterColumn<int>(
                name: "ExecutionId",
                table: "ExecutionLines",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_ExecutionLines_Executions_ExecutionId",
                table: "ExecutionLines",
                column: "ExecutionId",
                principalTable: "Executions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
