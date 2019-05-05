using Microsoft.EntityFrameworkCore.Migrations;

namespace DeathClock.Persistence.Migrations
{
    public partial class KnownAsToBase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImdbUrl",
                table: "BasePerson");

            migrationBuilder.AlterColumn<string>(
                name: "KnownFor",
                table: "BasePerson",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "BasePerson",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Url",
                table: "BasePerson");

            migrationBuilder.AlterColumn<string>(
                name: "KnownFor",
                table: "BasePerson",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "ImdbUrl",
                table: "BasePerson",
                maxLength: 200,
                nullable: true);
        }
    }
}
