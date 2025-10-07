using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bank.API.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitBank : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BankInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rating = table.Column<double>(type: "float", nullable: false),
                    HasLicense = table.Column<bool>(type: "bit", nullable: false),
                    BankFounderFullName = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    BankDirectorFullName = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Capitalization = table.Column<double>(type: "float", nullable: false),
                    EmployeesCount = table.Column<int>(type: "int", nullable: false),
                    BlockedClientsCount = table.Column<int>(type: "int", nullable: false),
                    ClientsCount = table.Column<int>(type: "int", nullable: false),
                    ActiveClientsCount = table.Column<int>(type: "int", nullable: false),
                    WebsiteUrl = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ContactPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    LegalAddress = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PercentageCommissionForBuyingCurrency = table.Column<double>(type: "float", nullable: false),
                    PercentageCommissionForSellingCurrency = table.Column<double>(type: "float", nullable: false),
                    SwiftCode = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    MfoCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TaxId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ContactEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankInfo", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BankInfo");
        }
    }
}
