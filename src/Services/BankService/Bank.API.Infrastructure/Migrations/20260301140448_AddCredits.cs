using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bank.API.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCredits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserCredits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreditTariffId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TermMonths = table.Column<int>(type: "int", nullable: false),
                    InterestRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    MonthlyPayment = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RemainingDebt = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CurrentMonthAmountDue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CurrentPaymentDate = table.Column<DateOnly>(type: "date", nullable: true),
                    AmountToClose = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsClosed = table.Column<bool>(type: "bit", nullable: false),
                    CloseTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCredits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserCredits_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserCredits_CreditTariffs_CreditTariffId",
                        column: x => x.CreditTariffId,
                        principalTable: "CreditTariffs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserCredits_CreditTariffId",
                table: "UserCredits",
                column: "CreditTariffId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCredits_UserId",
                table: "UserCredits",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserCredits");
        }
    }
}
