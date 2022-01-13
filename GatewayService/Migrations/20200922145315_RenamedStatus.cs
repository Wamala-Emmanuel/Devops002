using Microsoft.EntityFrameworkCore.Migrations;

namespace GatewayService.Migrations
{
    public partial class RenamedStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                newName: "RequestStatus",
                table: "Requests");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                newName: "Status",
                name: "RequestStatus",
                table: "Requests");
        }
    }
}
