using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebRecomendationControlApp.Data.Migrations
{
    public partial class AddComments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReviewComments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CommentedReviewId = table.Column<int>(type: "int", nullable: false),
                    CommentatorId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReviewComments_AspNetUsers_CommentatorId",
                        column: x => x.CommentatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReviewComments_Reviews_CommentedReviewId",
                        column: x => x.CommentedReviewId,
                        principalTable: "Reviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReviewComments_CommentatorId",
                table: "ReviewComments",
                column: "CommentatorId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewComments_CommentedReviewId",
                table: "ReviewComments",
                column: "CommentedReviewId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReviewComments");
        }
    }
}
