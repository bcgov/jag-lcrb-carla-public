using Microsoft.EntityFrameworkCore.Migrations;

namespace Gov.Lclb.Cllb.Public.Migrations
{
    public partial class postmodelchange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "clientId",
                table: "PostSurveyResults",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "clientId",
                table: "PostSurveyResults");
        }
    }
}
