using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Transactions.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTransactionAdditionalFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CurrencyExchangeCommission",
                table: "Transactions",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "GetterCurrency",
                table: "Transactions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SenderCurrency",
                table: "Transactions",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrencyExchangeCommission",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "GetterCurrency",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "SenderCurrency",
                table: "Transactions");
        }
    }
}
