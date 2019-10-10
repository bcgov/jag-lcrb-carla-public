using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Gov.Lclb.Cllb.Public.Migrations
{
    public partial class featurevotewidget : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VoteQuestions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Question = table.Column<string>(maxLength: 512, nullable: true),
                    Slug = table.Column<string>(nullable: true),
                    Title = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoteQuestions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VoteOptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DisplayOrder = table.Column<int>(nullable: false),
                    Option = table.Column<string>(maxLength: 512, nullable: true),
                    TotalVotes = table.Column<int>(nullable: false),
                    VoteQuestionId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoteOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VoteOptions_VoteQuestions_VoteQuestionId",
                        column: x => x.VoteQuestionId,
                        principalTable: "VoteQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VoteOptions_VoteQuestionId",
                table: "VoteOptions",
                column: "VoteQuestionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VoteOptions");

            migrationBuilder.DropTable(
                name: "VoteQuestions");
        }
    }
}
