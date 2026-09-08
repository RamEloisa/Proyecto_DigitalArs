using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalArs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "ID_User",
                keyValue: 1,
                column: "Password_Hasheada",
                value: "$2a$12$czX6t8AeyKgsBuM9F8DPo.J7an5VhqpJz34adBxZQjUEP5QhGC3QG");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "ID_User",
                keyValue: 2,
                column: "Password_Hasheada",
                value: "$2a$12$PN0hzgjrxIA789l5jXC2Euvfv.yNMYITgKuDuEwg9qs.Z7tpvY8qy");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "ID_User",
                keyValue: 3,
                column: "Password_Hasheada",
                value: "$2a$12$PN0hzgjrxIA789l5jXC2Euvfv.yNMYITgKuDuEwg9qs.Z7tpvY8qy");
        }
    }
}
