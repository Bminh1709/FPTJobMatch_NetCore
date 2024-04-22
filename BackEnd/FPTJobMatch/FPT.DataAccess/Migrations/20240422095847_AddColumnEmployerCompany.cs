using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FPT.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnEmployerCompany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HelpTypes_AspNetUsers_AdminId",
                table: "HelpTypes");

            migrationBuilder.AlterColumn<string>(
                name: "AdminId",
                table: "HelpTypes",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "EmployerId",
                table: "Companies",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Companies_EmployerId",
                table: "Companies",
                column: "EmployerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_AspNetUsers_EmployerId",
                table: "Companies",
                column: "EmployerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_HelpTypes_AspNetUsers_AdminId",
                table: "HelpTypes",
                column: "AdminId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Companies_AspNetUsers_EmployerId",
                table: "Companies");

            migrationBuilder.DropForeignKey(
                name: "FK_HelpTypes_AspNetUsers_AdminId",
                table: "HelpTypes");

            migrationBuilder.DropIndex(
                name: "IX_Companies_EmployerId",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "EmployerId",
                table: "Companies");

            migrationBuilder.AlterColumn<string>(
                name: "AdminId",
                table: "HelpTypes",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_HelpTypes_AspNetUsers_AdminId",
                table: "HelpTypes",
                column: "AdminId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
