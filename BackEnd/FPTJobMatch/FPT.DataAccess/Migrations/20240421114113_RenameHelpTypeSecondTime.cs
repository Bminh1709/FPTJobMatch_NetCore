using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FPT.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RenameHelpTypeSecondTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HelpArticles_HelpCategories_HelpTypeId",
                table: "HelpArticles");

            migrationBuilder.DropForeignKey(
                name: "FK_HelpCategories_AspNetUsers_AdminId",
                table: "HelpCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HelpCategories",
                table: "HelpCategories");

            migrationBuilder.RenameTable(
                name: "HelpCategories",
                newName: "HelpTypes");

            migrationBuilder.RenameIndex(
                name: "IX_HelpCategories_AdminId",
                table: "HelpTypes",
                newName: "IX_HelpTypes_AdminId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HelpTypes",
                table: "HelpTypes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_HelpArticles_HelpTypes_HelpTypeId",
                table: "HelpArticles",
                column: "HelpTypeId",
                principalTable: "HelpTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HelpTypes_AspNetUsers_AdminId",
                table: "HelpTypes",
                column: "AdminId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HelpArticles_HelpTypes_HelpTypeId",
                table: "HelpArticles");

            migrationBuilder.DropForeignKey(
                name: "FK_HelpTypes_AspNetUsers_AdminId",
                table: "HelpTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HelpTypes",
                table: "HelpTypes");

            migrationBuilder.RenameTable(
                name: "HelpTypes",
                newName: "HelpCategories");

            migrationBuilder.RenameIndex(
                name: "IX_HelpTypes_AdminId",
                table: "HelpCategories",
                newName: "IX_HelpCategories_AdminId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HelpCategories",
                table: "HelpCategories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_HelpArticles_HelpCategories_HelpTypeId",
                table: "HelpArticles",
                column: "HelpTypeId",
                principalTable: "HelpCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HelpCategories_AspNetUsers_AdminId",
                table: "HelpCategories",
                column: "AdminId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
