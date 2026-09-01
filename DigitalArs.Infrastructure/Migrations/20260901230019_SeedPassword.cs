using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalArs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedPassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "ID_User",
                keyValue: 1,
                column: "Password_Hasheada",
                value: "$2a$11$A2DjUB/.5foGtT8HgKe2BeORnMfhV/8qz0zGvQ41FeZiHvCTw8hbm");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "ID_User",
                keyValue: 2,
                column: "Password_Hasheada",
                value: "$2a$11$bTmDVSy5yvKsEmvUvYhfbeJnnjfi45oDq/4l88jhhqc6IGBE.gS0K");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "ID_User",
                keyValue: 3,
                column: "Password_Hasheada",
                value: "$2a$11$vG3KYGpGSU7FXz27zemMC.KRSDLLetL2WHuc4fdroAtTqTdqjEzzy");
        }
    }
}
