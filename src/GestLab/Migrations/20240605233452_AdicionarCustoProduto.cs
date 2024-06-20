using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestLab.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarCustoProduto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Custo",
                table: "Produto",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Custo",
                table: "Produto");
        }
    }
}
