using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GmailCleaner.Migrations
{
    /// <inheritdoc />
    public partial class Fix1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GCUser",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GmailId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsAdmin = table.Column<bool>(type: "bit", nullable: false),
                    Usages = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GCUser", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "GCMessage",
                columns: table => new
                {
                    MessageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MessageGmailId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Snippet = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UnsubscribeLink = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    From = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GCMessage", x => x.MessageId);
                    table.ForeignKey(
                        name: "FK_GCMessage_GCUser",
                        column: x => x.UserId,
                        principalTable: "GCUser",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "GCUserToken",
                columns: table => new
                {
                    UserTokenId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    AccessToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpiresOn = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GCUserToken", x => x.UserTokenId);
                    table.ForeignKey(
                        name: "FK_GCUserToken_GCUser",
                        column: x => x.UserId,
                        principalTable: "GCUser",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_GCMessage_UserId",
                table: "GCMessage",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_GCUser_GmailId",
                table: "GCUser",
                column: "GmailId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GCUserToken_UserId",
                table: "GCUserToken",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GCMessage");

            migrationBuilder.DropTable(
                name: "GCUserToken");

            migrationBuilder.DropTable(
                name: "GCUser");
        }
    }
}
