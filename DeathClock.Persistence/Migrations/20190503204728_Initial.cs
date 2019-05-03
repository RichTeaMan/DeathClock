using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DeathClock.Persistence.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TmdbPersons",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BirthDate = table.Column<DateTime>(nullable: false),
                    DeathDate = table.Column<DateTime>(nullable: true),
                    IsDead = table.Column<bool>(nullable: false),
                    Title = table.Column<string>(maxLength: 200, nullable: false),
                    ImdbUrl = table.Column<string>(maxLength: 200, nullable: false),
                    TmdbId = table.Column<int>(nullable: false),
                    KnownFor = table.Column<string>(nullable: false),
                    Popularity = table.Column<double>(nullable: false),
                    DataSet = table.Column<string>(maxLength: 10, nullable: false),
                    RecordedDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TmdbPersons", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TmdbPersons");
        }
    }
}
