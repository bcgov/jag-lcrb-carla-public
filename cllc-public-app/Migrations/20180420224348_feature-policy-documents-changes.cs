using Microsoft.EntityFrameworkCore.Migrations;

namespace Gov.Lclb.Cllb.Public.Migrations
{
    public partial class featurepolicydocumentschanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Intro",
                table: "PolicyDocuments",
                newName: "MenuText");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "PolicyDocuments",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "PolicyDocuments");

            migrationBuilder.RenameColumn(
                name: "MenuText",
                table: "PolicyDocuments",
                newName: "Intro");
        }
    }
}
