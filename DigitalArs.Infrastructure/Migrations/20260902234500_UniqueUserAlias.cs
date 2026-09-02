using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalArs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UniqueUserAlias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Alias duplicados (p. ej. ram.ram): se conserva el de menor ID_User y se sufija el resto.
            migrationBuilder.Sql("""
                WITH Duplicates AS (
                    SELECT ID_User, Alias,
                           ROW_NUMBER() OVER (PARTITION BY Alias ORDER BY ID_User) AS rn
                    FROM Users
                )
                UPDATE u
                SET Alias = LEFT(u.Alias, 50 - LEN(CAST(u.ID_User AS varchar(11))) - 1)
                            + '.' + CAST(u.ID_User AS varchar(11))
                FROM Users u
                INNER JOIN Duplicates d ON u.ID_User = d.ID_User
                WHERE d.rn > 1;
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Alias",
                table: "Users",
                column: "Alias",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Alias",
                table: "Users");
        }
    }
}
