using Microsoft.EntityFrameworkCore.Migrations;

namespace TCC.Data.Migrations
{
    public partial class Atualizacao_no_produto : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Periodicidade_das_consultas",
                table: "Produtos");

            migrationBuilder.AddColumn<string>(
                name: "Token",
                table: "Produtos",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Token",
                table: "Produtos");

            migrationBuilder.AddColumn<int>(
                name: "Periodicidade_das_consultas",
                table: "Produtos",
                nullable: false,
                defaultValue: 0);
        }
    }
}
