using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DigitalArs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "ID_Role", "Name" },
                values: new object[,]
                {
                    { 1, "Admin" },
                    { 2, "User" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "ID_User", "Alias", "DNI", "Email", "Full_Name", "ID_Role", "Password_Hasheada" },
                values: new object[,]
                {
                    { 1, "admin.digitalars", "30111222", "admin@digitalars.com", "Admin Principal", 1, "$2a$12$czX6t8AeyKgsBuM9F8DPo.J7an5VhqpJz34adBxZQjUEP5QhGC3QG" },
                    { 2, "juan.perez", "35222333", "juan.perez@digitalars.com", "Juan Perez", 2, "$2a$12$PN0hzgjrxIA789l5jXC2Euvfv.yNMYITgKuDuEwg9qs.Z7tpvY8qy" },
                    { 3, "maria.gomez", "36333444", "maria.gomez@digitalars.com", "Maria Gomez", 2, "$2a$12$PN0hzgjrxIA789l5jXC2Euvfv.yNMYITgKuDuEwg9qs.Z7tpvY8qy" }
                });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "ID_Account", "Date", "ID_User", "Name", "Price" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Cuenta Admin", 10000m },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "Cuenta Juan", 5000m },
                    { 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "Cuenta Maria", 7500m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "ID_Account",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "ID_Account",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "ID_Account",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "ID_User",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "ID_User",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "ID_User",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "ID_Role",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "ID_Role",
                keyValue: 2);
        }
    }
}
