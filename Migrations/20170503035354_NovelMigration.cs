using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LMS_Project.Migrations
{
    public partial class NovelMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Chapters",
                columns: table => new
                {
                    ChapterId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Content = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    NumberInEpisode = table.Column<int>(nullable: false),
                    WebAddress = table.Column<string>(nullable: true),
                    WebId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chapters", x => x.ChapterId);
                });

            migrationBuilder.CreateTable(
                name: "Episodes",
                columns: table => new
                {
                    EpisodeId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Image = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Episodes", x => x.EpisodeId);
                });

            migrationBuilder.CreateTable(
                name: "EpisodeDetails",
                columns: table => new
                {
                    ChapterId = table.Column<int>(nullable: false),
                    EpisodeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EpisodeDetails", x => new { x.ChapterId, x.EpisodeId });
                });

            migrationBuilder.CreateTable(
                name: "Novels",
                columns: table => new
                {
                    NovelId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ImageUrl = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    Author = table.Column<string>(nullable: true),
                    Summany = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Novels", x => x.NovelId);
                });

            migrationBuilder.CreateTable(
                name: "NovelDetails",
                columns: table => new
                {
                    NovelId = table.Column<int>(nullable: false),
                    EpisodeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NovelDetails", x => new { x.NovelId, x.EpisodeId });
                });

            migrationBuilder.CreateTable(
                name: "WebDetails",
                columns: table => new
                {
                    WebId = table.Column<int>(nullable: false),
                    NovelId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebDetails", x => new { x.WebId, x.NovelId });
                });

            migrationBuilder.CreateTable(
                name: "WebSourses",
                columns: table => new
                {
                    WebId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Address = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebSourses", x => x.WebId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Chapters");

            migrationBuilder.DropTable(
                name: "Episodes");

            migrationBuilder.DropTable(
                name: "EpisodeDetails");

            migrationBuilder.DropTable(
                name: "Novels");

            migrationBuilder.DropTable(
                name: "NovelDetails");

            migrationBuilder.DropTable(
                name: "WebDetails");

            migrationBuilder.DropTable(
                name: "WebSourses");
        }
    }
}
