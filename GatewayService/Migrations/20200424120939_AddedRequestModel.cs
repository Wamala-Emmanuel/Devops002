using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GatewayService.Migrations
{
    public partial class AddedRequestModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Requests",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ReferenceNumber = table.Column<int>(nullable: false),
                    Initiator = table.Column<string>(nullable: true),
                    ParticipantId = table.Column<Guid>(nullable: true),
                    ReceivedAt = table.Column<DateTime>(nullable: false),
                    SubmittedAt = table.Column<DateTime>(nullable: false),
                    ReceivedFromNira = table.Column<DateTime>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Result = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    CardNumber = table.Column<string>(nullable: true),
                    Nin = table.Column<string>(nullable: true),
                    DateOfBirth = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requests", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Requests");
        }
    }
}
