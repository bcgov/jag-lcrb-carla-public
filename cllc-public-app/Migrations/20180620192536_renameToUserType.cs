using Microsoft.EntityFrameworkCore.Migrations;

namespace Gov.Lclb.Cllb.Public.Migrations
{
    public partial class renameToUserType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SmAuthorizationDirectory",
                table: "Users",
                newName: "UserType");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserType",
                table: "Users",
                newName: "SmAuthorizationDirectory");
        }
    }
}
