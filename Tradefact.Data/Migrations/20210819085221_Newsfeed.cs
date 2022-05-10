using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tradefact.Data.Migrations
{
    public partial class Newsfeed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "CargoItems");

            migrationBuilder.CreateTable(
                name: "NewsFeeds",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedByUser = table.Column<string>(maxLength: 128, nullable: true),
                    CreationDateInternal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastChangeUser = table.Column<string>(maxLength: 128, nullable: true),
                    LastModifiedOnInternal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WeekNo = table.Column<int>(nullable: false),
                    Type = table.Column<string>(maxLength: 32, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsFeeds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NewsFeedSections",
                columns: table => new
                {
                    NewsFeedId = table.Column<Guid>(nullable: false),
                    Reference = table.Column<string>(maxLength: 36, nullable: false),
                    Title = table.Column<string>(maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsFeedSections", x => new { x.NewsFeedId, x.Reference });
                    table.ForeignKey(
                        name: "FK_NewsFeedSections_NewsFeeds_NewsFeedId",
                        column: x => x.NewsFeedId,
                        principalTable: "NewsFeeds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NewsFeedItems",
                columns: table => new
                {
                    NewsFeedId = table.Column<Guid>(nullable: false),
                    NewsSectionReference = table.Column<string>(maxLength: 36, nullable: false),
                    SeqNo = table.Column<int>(nullable: false),
                    Text = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsFeedItems", x => new { x.NewsFeedId, x.NewsSectionReference, x.SeqNo });
                    table.ForeignKey(
                        name: "FK_NewsFeedItems_NewsFeedSections_NewsFeedId_NewsSectionReference",
                        columns: x => new { x.NewsFeedId, x.NewsSectionReference },
                        principalTable: "NewsFeedSections",
                        principalColumns: new[] { "NewsFeedId", "Reference" },
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NewsFeedItems");

            migrationBuilder.DropTable(
                name: "NewsFeedSections");

            migrationBuilder.DropTable(
                name: "NewsFeeds");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "CargoItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
