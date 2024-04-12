using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FPT.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddForeignKeyCompanyApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Companies_AspNetUsers_EmployerId",
                table: "Companies");

            migrationBuilder.DropIndex(
                name: "IX_Companies_EmployerId",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "EmployerId",
                table: "Companies");

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "AspNetUsers",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CompanyId",
                table: "AspNetUsers",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Companies_CompanyId",
                table: "AspNetUsers",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Companies_CompanyId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_CompanyId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "AspNetUsers");

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
        }
    }
}
