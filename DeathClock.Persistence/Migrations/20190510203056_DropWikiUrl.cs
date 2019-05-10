using Microsoft.EntityFrameworkCore.Migrations;

namespace DeathClock.Persistence.Migrations
{
    public partial class DropWikiUrl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WikipediaUrl",
                table: "BasePerson");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WikipediaUrl",
                table: "BasePerson",
                maxLength: 200,
                nullable: true);
        }
    }
}
