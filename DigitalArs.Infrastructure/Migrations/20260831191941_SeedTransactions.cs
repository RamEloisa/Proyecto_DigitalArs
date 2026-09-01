using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DigitalArs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Transactions",
                columns: new[] { "ID_Transaction", "Amount", "Date_Transaction", "ID_Account", "Type" },
                values: new object[,]
                {
                    { 1, 2000m, new DateTime(2026, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 0 },
                    { 2, 1500m, new DateTime(2026, 1, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2 },
                    { 3, 1500m, new DateTime(2026, 1, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "ID_Transaction",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "ID_Transaction",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "ID_Transaction",
                keyValue: 3);
        }
    }
}
