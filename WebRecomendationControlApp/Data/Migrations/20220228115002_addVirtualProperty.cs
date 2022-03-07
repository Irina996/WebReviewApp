using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebRecomendationControlApp.Data.Migrations
{
    public partial class addVirtualProperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_reviewTags_Reviews_ReviewId",
                table: "reviewTags");

            migrationBuilder.AlterColumn<int>(
                name: "ReviewId",
                table: "reviewTags",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_reviewTags_Reviews_ReviewId",
                table: "reviewTags",
                column: "ReviewId",
                principalTable: "Reviews",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_reviewTags_Reviews_ReviewId",
                table: "reviewTags");

            migrationBuilder.AlterColumn<int>(
                name: "ReviewId",
                table: "reviewTags",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_reviewTags_Reviews_ReviewId",
                table: "reviewTags",
                column: "ReviewId",
                principalTable: "Reviews",
                principalColumn: "Id");
        }
    }
}
