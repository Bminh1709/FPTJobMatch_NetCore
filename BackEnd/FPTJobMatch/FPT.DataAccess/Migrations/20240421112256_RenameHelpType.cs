using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FPT.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RenameHelpType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HelpArticles_HelpCategories_HelpCategoryId",
                table: "HelpArticles");

            migrationBuilder.RenameColumn(
                name: "HelpCategoryId",
                table: "HelpArticles",
                newName: "HelpTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_HelpArticles_HelpCategoryId",
                table: "HelpArticles",
                newName: "IX_HelpArticles_HelpTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_HelpArticles_HelpCategories_HelpTypeId",
                table: "HelpArticles",
                column: "HelpTypeId",
                principalTable: "HelpCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HelpArticles_HelpCategories_HelpTypeId",
                table: "HelpArticles");

            migrationBuilder.RenameColumn(
                name: "HelpTypeId",
                table: "HelpArticles",
                newName: "HelpCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_HelpArticles_HelpTypeId",
                table: "HelpArticles",
                newName: "IX_HelpArticles_HelpCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_HelpArticles_HelpCategories_HelpCategoryId",
                table: "HelpArticles",
                column: "HelpCategoryId",
                principalTable: "HelpCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
