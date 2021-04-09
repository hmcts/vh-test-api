using Microsoft.EntityFrameworkCore.Migrations;
using TestApi.DAL.SeedData;

namespace TestApi.DAL.Migrations
{
    public partial class SeedEjudUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            new SeedEjudUsersData().Up(migrationBuilder);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            new SeedEjudUsersData().Down(migrationBuilder);
        }
    }
}
