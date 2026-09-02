using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalArs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeedPasswords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "ID_User",
                keyValue: 1,
                column: "Password_Hasheada",
                value: "$2a$11$4UhybbztKrzx7HvDlvNVaurxyqU5U4hT5.kGJGXzWCtWnKQdTxyQS");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "ID_User",
                keyValue: 2,
                column: "Password_Hasheada",
                value: "$2a$11$lYxAthsE/KgadR48MPwD7eQJ6o6qGitI5A9fCLXTUhKmimbT6G7Wa");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "ID_User",
                keyValue: 3,
                column: "Password_Hasheada",
                value: "$2a$11$lYxAthsE/KgadR48MPwD7eQJ6o6qGitI5A9fCLXTUhKmimbT6G7Wa");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "ID_User",
                keyValue: 1,
                column: "Password_Hasheada",
                value: "$2a$11$Lcc/.JSUqr0pWtLG9/SIwe0R3J7uk.QwAiJ0i4Vws3odRsPRa28uy");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "ID_User",
                keyValue: 2,
                column: "Password_Hasheada",
                value: "$2a$11$4/qtspjvQA4wq8df1/6oq..O.REndayYq/MGD5W8I0uQOtpO21icW");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "ID_User",
                keyValue: 3,
                column: "Password_Hasheada",
                value: "$2a$11$4/qtspjvQA4wq8df1/6oq..O.REndayYq/MGD5W8I0uQOtpO21icW");
        }
    }
}
