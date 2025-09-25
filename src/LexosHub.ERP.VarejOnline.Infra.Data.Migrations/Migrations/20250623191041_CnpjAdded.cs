using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LexosHub.ERP.VarejOnline.Infra.Data.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class CnpjAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Cnpj",
                table: "Integration",
                type: "varchar(1024)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cnpj",
                table: "Integration");
        }
    }
}
