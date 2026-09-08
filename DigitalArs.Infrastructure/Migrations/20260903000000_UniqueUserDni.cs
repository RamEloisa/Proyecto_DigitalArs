using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalArs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UniqueUserDni : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // DNI duplicados: se conserva el de menor ID_User y se sufija el resto (sigue numérico).
            migrationBuilder.Sql("""
                WITH Duplicates AS (
                    SELECT ID_User, DNI,
                           ROW_NUMBER() OVER (PARTITION BY DNI ORDER BY ID_User) AS rn
                    FROM Users
                )
                UPDATE u
                SET DNI = LEFT(u.DNI, 20 - LEN(CAST(u.ID_User AS varchar(11))))
                          + CAST(u.ID_User AS varchar(11))
                FROM Users u
                INNER JOIN Duplicates d ON u.ID_User = d.ID_User
                WHERE d.rn > 1;
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Users_DNI",
                table: "Users",
                column: "DNI",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_DNI",
                table: "Users");
        }
    }
}
