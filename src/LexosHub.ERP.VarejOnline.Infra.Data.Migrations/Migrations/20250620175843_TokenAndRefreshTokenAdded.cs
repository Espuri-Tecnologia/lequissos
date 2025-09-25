using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LexosHub.ERP.VarejOnline.Infra.Data.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class TokenAndRefreshTokenAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Integration",
                type: "varchar(1024)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "Integration",
                type: "varchar(1024)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Token",
                table: "Integration",
                type: "varchar(1024)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "User",
                table: "Integration",
                type: "varchar(1024)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "Integration");

            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "Integration");

            migrationBuilder.DropColumn(
                name: "Token",
                table: "Integration");

            migrationBuilder.DropColumn(
                name: "User",
                table: "Integration");
        }
    }
}
