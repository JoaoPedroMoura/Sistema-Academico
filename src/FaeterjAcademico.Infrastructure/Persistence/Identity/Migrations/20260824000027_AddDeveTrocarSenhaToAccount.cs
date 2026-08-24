using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FaeterjAcademico.Infrastructure.Persistence.Identity.Migrations
{
    /// <inheritdoc />
    public partial class AddDeveTrocarSenhaToAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DeveTrocarSenha",
                schema: "identity",
                table: "Accounts",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeveTrocarSenha",
                schema: "identity",
                table: "Accounts");
        }
    }
}
