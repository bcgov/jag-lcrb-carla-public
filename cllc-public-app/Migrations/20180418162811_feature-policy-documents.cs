using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Gov.Lclb.Cllb.Public.Migrations
{
    public partial class featurepolicydocuments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PolicyDocuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Body = table.Column<string>(nullable: true),
                    DisplayOrder = table.Column<int>(nullable: false),
                    Intro = table.Column<string>(nullable: true),
                    Slug = table.Column<string>(nullable: true),
                    Title = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PolicyDocuments", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PolicyDocuments");
        }
    }
}
