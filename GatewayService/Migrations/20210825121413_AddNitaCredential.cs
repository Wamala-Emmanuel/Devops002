using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GatewayService.Migrations
{
    public partial class AddNitaCredential : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NitaCredentials",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ClientKey = table.Column<string>(nullable: false),
                    ClientSecret = table.Column<string>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NitaCredentials", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NitaCredentials");
        }
    }
}
