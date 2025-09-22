using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bank.API.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixCurrencyFieldName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EnableCurency",
                table: "CardTariffs",
                newName: "EnableCurrency");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EnableCurrency",
                table: "CardTariffs",
                newName: "EnableCurency");
        }
    }
}
