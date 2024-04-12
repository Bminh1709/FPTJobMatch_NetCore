using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FPT.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ChangeNameStatusTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicantCVs_AccountStatuses_StatusId",
                table: "ApplicantCVs");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AccountStatuses_StatusId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Companies_AspNetUsers_EmployerId",
                table: "Companies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AccountStatuses",
                table: "AccountStatuses");

            migrationBuilder.RenameTable(
                name: "AccountStatuses",
                newName: "Statuses");

            migrationBuilder.AlterColumn<string>(
                name: "EmployerId",
                table: "Companies",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Statuses",
                table: "Statuses",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicantCVs_Statuses_StatusId",
                table: "ApplicantCVs",
                column: "StatusId",
                principalTable: "Statuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Statuses_StatusId",
                table: "AspNetUsers",
                column: "StatusId",
                principalTable: "Statuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_AspNetUsers_EmployerId",
                table: "Companies",
                column: "EmployerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicantCVs_Statuses_StatusId",
                table: "ApplicantCVs");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Statuses_StatusId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Companies_AspNetUsers_EmployerId",
                table: "Companies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Statuses",
                table: "Statuses");

            migrationBuilder.RenameTable(
                name: "Statuses",
                newName: "AccountStatuses");

            migrationBuilder.AlterColumn<string>(
                name: "EmployerId",
                table: "Companies",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AccountStatuses",
                table: "AccountStatuses",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicantCVs_AccountStatuses_StatusId",
                table: "ApplicantCVs",
                column: "StatusId",
                principalTable: "AccountStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AccountStatuses_StatusId",
                table: "AspNetUsers",
                column: "StatusId",
                principalTable: "AccountStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_AspNetUsers_EmployerId",
                table: "Companies",
                column: "EmployerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
