using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalArs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIsActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "ID_User",
                keyValue: 1,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "ID_User",
                keyValue: 2,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "ID_User",
                keyValue: 3,
                column: "IsActive",
                value: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Users");
        }
    }
}
