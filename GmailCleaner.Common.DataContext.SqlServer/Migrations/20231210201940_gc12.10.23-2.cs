using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GmailCleaner.Common.DataContext.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class gc1210232 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAdmin",
                table: "GCUser",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Usages",
                table: "GCUser",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAdmin",
                table: "GCUser");

            migrationBuilder.DropColumn(
                name: "Usages",
                table: "GCUser");
        }
    }
}
