using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Gov.Lclb.Cllb.Public.Migrations
{
    public partial class featureuserrefactoring : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Users_UserId",
                table: "UserRoles");

            migrationBuilder.RenameColumn(
                name: "Guid",
                table: "Users",
                newName: "SiteMinderGuid");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Users",
                newName: "ContactId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "UserRoles",
                newName: "UserContactId");

            migrationBuilder.RenameIndex(
                name: "IX_UserRoles_UserId",
                table: "UserRoles",
                newName: "IX_UserRoles_UserContactId");

            migrationBuilder.AddColumn<Guid>(
                name: "AccountId",
                table: "Users",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Users_UserContactId",
                table: "UserRoles",
                column: "UserContactId",
                principalTable: "Users",
                principalColumn: "ContactId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Users_UserContactId",
                table: "UserRoles");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "SiteMinderGuid",
                table: "Users",
                newName: "Guid");

            migrationBuilder.RenameColumn(
                name: "ContactId",
                table: "Users",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "UserContactId",
                table: "UserRoles",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserRoles_UserContactId",
                table: "UserRoles",
                newName: "IX_UserRoles_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Users_UserId",
                table: "UserRoles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
