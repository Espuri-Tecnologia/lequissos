using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LexosHub.ERP.VarejoOnline.Infra.Data.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class UserAndPasswordRemoved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "Integration");

            migrationBuilder.DropColumn(
                name: "User",
                table: "Integration");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Integration",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "User",
                table: "Integration",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
