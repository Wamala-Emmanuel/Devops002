using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GatewayService.Migrations
{
    public partial class RenameCredentialModelProperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExipresOn",
                newName: "ExpiresOn",
                table: "Credentials");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                newName: "ExipresOn",
                name: "ExpiresOn",
                table: "Credentials");
        }
    }
}
