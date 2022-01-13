using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GatewayService.Migrations
{
    public partial class AddInitiatorProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InitiatorEmail",
                table: "Requests",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "InitiatorId",
                table: "Requests",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InitiatorEmail",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "InitiatorId",
                table: "Requests");
        }
    }
}
