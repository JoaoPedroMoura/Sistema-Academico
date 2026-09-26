using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FaeterjAcademico.Infrastructure.Persistence.Academico.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentosPessoais : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Bairro",
                table: "Professores",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cep",
                table: "Professores",
                type: "character varying(8)",
                maxLength: 8,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cidade",
                table: "Professores",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Complemento",
                table: "Professores",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cpf",
                table: "Professores",
                type: "character varying(11)",
                maxLength: 11,
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "DataNascimento",
                table: "Professores",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LattesUrl",
                table: "Professores",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Logradouro",
                table: "Professores",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Naturalidade",
                table: "Professores",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NomeMae",
                table: "Professores",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NomePai",
                table: "Professores",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Numero",
                table: "Professores",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "RgDataEmissao",
                table: "Professores",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RgNumero",
                table: "Professores",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RgOrgaoEmissor",
                table: "Professores",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RgUf",
                table: "Professores",
                type: "character varying(2)",
                maxLength: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Titulacao",
                table: "Professores",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Uf",
                table: "Professores",
                type: "character varying(2)",
                maxLength: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Bairro",
                table: "Alunos",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cep",
                table: "Alunos",
                type: "character varying(8)",
                maxLength: 8,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CertificadoReservista",
                table: "Alunos",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cidade",
                table: "Alunos",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Complemento",
                table: "Alunos",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cpf",
                table: "Alunos",
                type: "character varying(11)",
                maxLength: 11,
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "DataNascimento",
                table: "Alunos",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EnsinoMedioAnoConclusao",
                table: "Alunos",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EnsinoMedioInstituicao",
                table: "Alunos",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Logradouro",
                table: "Alunos",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Naturalidade",
                table: "Alunos",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NomeMae",
                table: "Alunos",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NomePai",
                table: "Alunos",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Numero",
                table: "Alunos",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "RgDataEmissao",
                table: "Alunos",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RgNumero",
                table: "Alunos",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RgOrgaoEmissor",
                table: "Alunos",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RgUf",
                table: "Alunos",
                type: "character varying(2)",
                maxLength: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TituloEleitor",
                table: "Alunos",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Uf",
                table: "Alunos",
                type: "character varying(2)",
                maxLength: 2,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DocumentosAnexos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PessoaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Tipo = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    NomeArquivo = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TamanhoBytes = table.Column<long>(type: "bigint", nullable: false),
                    Conteudo = table.Column<byte[]>(type: "bytea", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentosAnexos", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentosAnexos_PessoaId",
                table: "DocumentosAnexos",
                column: "PessoaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentosAnexos");

            migrationBuilder.DropColumn(
                name: "Bairro",
                table: "Professores");

            migrationBuilder.DropColumn(
                name: "Cep",
                table: "Professores");

            migrationBuilder.DropColumn(
                name: "Cidade",
                table: "Professores");

            migrationBuilder.DropColumn(
                name: "Complemento",
                table: "Professores");

            migrationBuilder.DropColumn(
                name: "Cpf",
                table: "Professores");

            migrationBuilder.DropColumn(
                name: "DataNascimento",
                table: "Professores");

            migrationBuilder.DropColumn(
                name: "LattesUrl",
                table: "Professores");

            migrationBuilder.DropColumn(
                name: "Logradouro",
                table: "Professores");

            migrationBuilder.DropColumn(
                name: "Naturalidade",
                table: "Professores");

            migrationBuilder.DropColumn(
                name: "NomeMae",
                table: "Professores");

            migrationBuilder.DropColumn(
                name: "NomePai",
                table: "Professores");

            migrationBuilder.DropColumn(
                name: "Numero",
                table: "Professores");

            migrationBuilder.DropColumn(
                name: "RgDataEmissao",
                table: "Professores");

            migrationBuilder.DropColumn(
                name: "RgNumero",
                table: "Professores");

            migrationBuilder.DropColumn(
                name: "RgOrgaoEmissor",
                table: "Professores");

            migrationBuilder.DropColumn(
                name: "RgUf",
                table: "Professores");

            migrationBuilder.DropColumn(
                name: "Titulacao",
                table: "Professores");

            migrationBuilder.DropColumn(
                name: "Uf",
                table: "Professores");

            migrationBuilder.DropColumn(
                name: "Bairro",
                table: "Alunos");

            migrationBuilder.DropColumn(
                name: "Cep",
                table: "Alunos");

            migrationBuilder.DropColumn(
                name: "CertificadoReservista",
                table: "Alunos");

            migrationBuilder.DropColumn(
                name: "Cidade",
                table: "Alunos");

            migrationBuilder.DropColumn(
                name: "Complemento",
                table: "Alunos");

            migrationBuilder.DropColumn(
                name: "Cpf",
                table: "Alunos");

            migrationBuilder.DropColumn(
                name: "DataNascimento",
                table: "Alunos");

            migrationBuilder.DropColumn(
                name: "EnsinoMedioAnoConclusao",
                table: "Alunos");

            migrationBuilder.DropColumn(
                name: "EnsinoMedioInstituicao",
                table: "Alunos");

            migrationBuilder.DropColumn(
                name: "Logradouro",
                table: "Alunos");

            migrationBuilder.DropColumn(
                name: "Naturalidade",
                table: "Alunos");

            migrationBuilder.DropColumn(
                name: "NomeMae",
                table: "Alunos");

            migrationBuilder.DropColumn(
                name: "NomePai",
                table: "Alunos");

            migrationBuilder.DropColumn(
                name: "Numero",
                table: "Alunos");

            migrationBuilder.DropColumn(
                name: "RgDataEmissao",
                table: "Alunos");

            migrationBuilder.DropColumn(
                name: "RgNumero",
                table: "Alunos");

            migrationBuilder.DropColumn(
                name: "RgOrgaoEmissor",
                table: "Alunos");

            migrationBuilder.DropColumn(
                name: "RgUf",
                table: "Alunos");

            migrationBuilder.DropColumn(
                name: "TituloEleitor",
                table: "Alunos");

            migrationBuilder.DropColumn(
                name: "Uf",
                table: "Alunos");
        }
    }
}
