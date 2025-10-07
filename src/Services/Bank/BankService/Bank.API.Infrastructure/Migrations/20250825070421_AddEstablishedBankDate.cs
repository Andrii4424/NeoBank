using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bank.API.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEstablishedBankDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EstablishedDate",
                table: "BankInfo",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstablishedDate",
                table: "BankInfo");
        }
    }
}
