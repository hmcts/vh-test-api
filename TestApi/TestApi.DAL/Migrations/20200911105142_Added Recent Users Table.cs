using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TestApi.DAL.Migrations
{
    public partial class AddedRecentUsersTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RecentUser",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Username = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecentUser", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecentUser_Username",
                table: "RecentUser",
                column: "Username",
                unique: true,
                filter: "[Username] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecentUser");
        }
    }
}
