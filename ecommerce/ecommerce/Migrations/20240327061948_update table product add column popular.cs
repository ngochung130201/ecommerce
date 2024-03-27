using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ecommerce.Migrations
{
    public partial class updatetableproductaddcolumnpopular : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "popular",
                table: "product",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "popular_text",
                table: "product",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "popular",
                table: "product");

            migrationBuilder.DropColumn(
                name: "popular_text",
                table: "product");
        }
    }
}
