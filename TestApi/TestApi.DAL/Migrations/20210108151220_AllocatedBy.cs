using Microsoft.EntityFrameworkCore.Migrations;

namespace TestApi.DAL.Migrations
{
    public partial class AllocatedBy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AllocatedBy",
                table: "Allocation",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllocatedBy",
                table: "Allocation");
        }
    }
}
