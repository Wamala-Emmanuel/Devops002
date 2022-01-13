using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GatewayService.Migrations
{
    public partial class UpdateCredentialModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Credentials",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExipresOn",
                table: "Credentials",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JobId",
                table: "Credentials",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Credentials");

            migrationBuilder.DropColumn(
                name: "ExipresOn",
                table: "Credentials");

            migrationBuilder.DropColumn(
                name: "JobId",
                table: "Credentials");
        }
    }
}
