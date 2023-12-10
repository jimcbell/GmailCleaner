using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GmailCleaner.Common.DataContext.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class gc1210231 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateIndex(
                name: "IX_GCMessage_UserId",
                table: "GCMessage",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GCMessage");
        }
    }
}
