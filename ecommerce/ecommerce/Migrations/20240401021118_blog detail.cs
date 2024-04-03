using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ecommerce.Migrations
{
    public partial class blogdetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_blog_detail_blog_id",
                table: "blog_detail");

            migrationBuilder.CreateIndex(
                name: "IX_blog_detail_blog_id",
                table: "blog_detail",
                column: "blog_id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_blog_detail_blog_id",
                table: "blog_detail");

            migrationBuilder.CreateIndex(
                name: "IX_blog_detail_blog_id",
                table: "blog_detail",
                column: "blog_id");
        }
    }
}
