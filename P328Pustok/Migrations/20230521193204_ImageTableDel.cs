using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace P328Pustok.Migrations
{
    public partial class ImageTableDel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookImages_Images_ImagesId",
                table: "BookImages");

            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropIndex(
                name: "IX_BookImages_ImagesId",
                table: "BookImages");

            migrationBuilder.DropColumn(
                name: "ImagesId",
                table: "BookImages");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "BookImages",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "BookImages");

            migrationBuilder.AddColumn<int>(
                name: "ImagesId",
                table: "BookImages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookImageId = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImgUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Images_BookImages_BookImageId",
                        column: x => x.BookImageId,
                        principalTable: "BookImages",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookImages_ImagesId",
                table: "BookImages",
                column: "ImagesId");

            migrationBuilder.CreateIndex(
                name: "IX_Images_BookImageId",
                table: "Images",
                column: "BookImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookImages_Images_ImagesId",
                table: "BookImages",
                column: "ImagesId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
