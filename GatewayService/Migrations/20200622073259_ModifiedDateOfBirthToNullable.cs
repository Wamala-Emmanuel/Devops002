using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GatewayService.Migrations
{
    public partial class ModifiedDateOfBirthToNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Requests");

            migrationBuilder.AlterColumn<string>(
                name: "Result",
                table: "Requests",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Initiator",
                table: "Requests",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfBirth",
                table: "Requests",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "GivenNames",
                table: "Requests",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherNames",
                table: "Requests",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Surname",
                table: "Requests",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GivenNames",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "OtherNames",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "Surname",
                table: "Requests");

            migrationBuilder.AlterColumn<string>(
                name: "Result",
                table: "Requests",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Initiator",
                table: "Requests",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfBirth",
                table: "Requests",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Requests",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
