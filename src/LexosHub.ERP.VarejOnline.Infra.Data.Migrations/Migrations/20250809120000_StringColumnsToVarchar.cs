using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LexosHub.ERP.VarejOnline.Infra.Data.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class StringColumnsToVarchar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Uuid",
                table: "Webhook",
                type: "varchar(1024)",
                maxLength: 1024,
                nullable: true,
                unicode: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "Webhook",
                type: "varchar(1024)",
                maxLength: 1024,
                nullable: false,
                unicode: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Types",
                table: "Webhook",
                type: "varchar(1024)",
                maxLength: 1024,
                nullable: false,
                unicode: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Event",
                table: "Webhook",
                type: "varchar(1024)",
                maxLength: 1024,
                nullable: false,
                unicode: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "JobId",
                table: "SyncProcessItem",
                type: "varchar(1024)",
                maxLength: 1024,
                nullable: true,
                unicode: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ExternalErpId",
                table: "SyncProcessItem",
                type: "varchar(1024)",
                maxLength: 1024,
                nullable: false,
                unicode: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "SyncProcessItem",
                type: "varchar(1024)",
                maxLength: 1024,
                nullable: false,
                unicode: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "AdditionalInfo",
                table: "SyncProcessItem",
                type: "varchar(1024)",
                maxLength: 1024,
                nullable: true,
                unicode: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AdditionalInfo",
                table: "SyncProcess",
                type: "varchar(1024)",
                maxLength: 1024,
                nullable: true,
                unicode: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "Integration",
                type: "varchar(1024)",
                maxLength: 1024,
                nullable: true,
                unicode: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "Integration",
                type: "varchar(1024)",
                maxLength: 1024,
                nullable: true,
                unicode: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Settings",
                table: "Integration",
                type: "varchar(1024)",
                maxLength: 1024,
                nullable: true,
                unicode: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RefreshToken",
                table: "Integration",
                type: "varchar(1024)",
                maxLength: 1024,
                nullable: true,
                unicode: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "HubKey",
                table: "Integration",
                type: "varchar(1024)",
                maxLength: 1024,
                nullable: true,
                unicode: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Cnpj",
                table: "Integration",
                type: "varchar(1024)",
                maxLength: 1024,
                nullable: true,
                unicode: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Uuid",
                table: "Webhook",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1024)",
                oldMaxLength: 1024,
                oldUnicode: false,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "Webhook",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(1024)",
                oldMaxLength: 1024,
                oldUnicode: false);

            migrationBuilder.AlterColumn<string>(
                name: "Types",
                table: "Webhook",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(1024)",
                oldMaxLength: 1024,
                oldUnicode: false);

            migrationBuilder.AlterColumn<string>(
                name: "Event",
                table: "Webhook",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(1024)",
                oldMaxLength: 1024,
                oldUnicode: false);

            migrationBuilder.AlterColumn<string>(
                name: "JobId",
                table: "SyncProcessItem",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1024)",
                oldMaxLength: 1024,
                oldUnicode: false,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ExternalErpId",
                table: "SyncProcessItem",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(1024)",
                oldMaxLength: 1024,
                oldUnicode: false);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "SyncProcessItem",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(1024)",
                oldMaxLength: 1024,
                oldUnicode: false);

            migrationBuilder.AlterColumn<string>(
                name: "AdditionalInfo",
                table: "SyncProcessItem",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1024)",
                oldMaxLength: 1024,
                oldUnicode: false,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AdditionalInfo",
                table: "SyncProcess",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1024)",
                oldMaxLength: 1024,
                oldUnicode: false,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "Integration",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1024)",
                oldMaxLength: 1024,
                oldUnicode: false,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "Integration",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1024)",
                oldMaxLength: 1024,
                oldUnicode: false,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Settings",
                table: "Integration",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1024)",
                oldMaxLength: 1024,
                oldUnicode: false,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RefreshToken",
                table: "Integration",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1024)",
                oldMaxLength: 1024,
                oldUnicode: false,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "HubKey",
                table: "Integration",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1024)",
                oldMaxLength: 1024,
                oldUnicode: false,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Cnpj",
                table: "Integration",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1024)",
                oldMaxLength: 1024,
                oldUnicode: false,
                oldNullable: true);
        }
    }
}
