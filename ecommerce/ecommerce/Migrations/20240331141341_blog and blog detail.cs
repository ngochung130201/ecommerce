using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ecommerce.Migrations
{
    public partial class blogandblogdetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "blog",
                columns: table => new
                {
                    blog_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    slug = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    created_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    image = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_blog", x => x.blog_id);
                });

            migrationBuilder.CreateTable(
                name: "blog_category",
                columns: table => new
                {
                    category_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    slug = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    created_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_blog_category", x => x.category_id);
                });

            migrationBuilder.CreateTable(
                name: "blog_detail",
                columns: table => new
                {
                    blog_detail_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    blog_id = table.Column<int>(type: "int", nullable: false),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_blog_detail", x => x.blog_detail_id);
                    table.ForeignKey(
                        name: "FK_blog_detail_blog_blog_id",
                        column: x => x.blog_id,
                        principalTable: "blog",
                        principalColumn: "blog_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BlogBlogCategory",
                columns: table => new
                {
                    BlogsBlogId = table.Column<int>(type: "int", nullable: false),
                    CategoriesCategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogBlogCategory", x => new { x.BlogsBlogId, x.CategoriesCategoryId });
                    table.ForeignKey(
                        name: "FK_BlogBlogCategory_blog_BlogsBlogId",
                        column: x => x.BlogsBlogId,
                        principalTable: "blog",
                        principalColumn: "blog_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BlogBlogCategory_blog_category_CategoriesCategoryId",
                        column: x => x.CategoriesCategoryId,
                        principalTable: "blog_category",
                        principalColumn: "category_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_blog_detail_blog_id",
                table: "blog_detail",
                column: "blog_id");

            migrationBuilder.CreateIndex(
                name: "IX_BlogBlogCategory_CategoriesCategoryId",
                table: "BlogBlogCategory",
                column: "CategoriesCategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "blog_detail");

            migrationBuilder.DropTable(
                name: "BlogBlogCategory");

            migrationBuilder.DropTable(
                name: "blog");

            migrationBuilder.DropTable(
                name: "blog_category");
        }
    }
}
