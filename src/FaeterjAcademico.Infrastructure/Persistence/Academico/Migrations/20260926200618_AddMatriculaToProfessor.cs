using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FaeterjAcademico.Infrastructure.Persistence.Academico.Migrations
{
    /// <inheritdoc />
    public partial class AddMatriculaToProfessor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Matricula",
                table: "Professores",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            // Professores já cadastrados: ano+semestre do cadastro + sequencial de 5 dígitos.
            migrationBuilder.Sql("""
                UPDATE "Professores" p SET "Matricula" = n.matricula
                FROM (
                    SELECT "Id",
                           to_char("CreatedAtUtc", 'YYYY')
                           || CASE WHEN extract(month FROM "CreatedAtUtc") <= 6 THEN '1' ELSE '2' END
                           || lpad(row_number() OVER (ORDER BY "CreatedAtUtc")::text, 5, '0') AS matricula
                    FROM "Professores"
                ) n
                WHERE p."Id" = n."Id";
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Professores_Matricula",
                table: "Professores",
                column: "Matricula",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Professores_Matricula",
                table: "Professores");

            migrationBuilder.DropColumn(
                name: "Matricula",
                table: "Professores");
        }
    }
}
