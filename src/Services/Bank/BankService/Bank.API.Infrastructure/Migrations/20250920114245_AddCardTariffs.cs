using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bank.API.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCardTariffs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CardTariffs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BankId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CardName = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    ValidityPeriod = table.Column<double>(type: "float", nullable: false),
                    MaxCreditLimit = table.Column<int>(type: "int", nullable: false),
                    EnabledPaymentSystems = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InterestRate = table.Column<double>(type: "float", nullable: true),
                    EnableCurency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AnnualMaintenanceCost = table.Column<int>(type: "int", nullable: false),
                    P2PInternalCommission = table.Column<double>(type: "float", nullable: false),
                    CardColor = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    BIN = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardTariffs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CardTariffs_BankInfo_BankId",
                        column: x => x.BankId,
                        principalTable: "BankInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CardTariffs_BankId",
                table: "CardTariffs",
                column: "BankId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CardTariffs");
        }
    }
}
