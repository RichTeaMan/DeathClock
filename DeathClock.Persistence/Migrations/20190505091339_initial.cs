using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DeathClock.Persistence.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BasePerson",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BirthDate = table.Column<DateTime>(nullable: false),
                    DeathDate = table.Column<DateTime>(nullable: true),
                    IsDead = table.Column<bool>(nullable: false),
                    Title = table.Column<string>(maxLength: 200, nullable: false),
                    DataSet = table.Column<string>(maxLength: 10, nullable: false),
                    RecordedDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    ImdbUrl = table.Column<string>(maxLength: 200, nullable: true),
                    TmdbId = table.Column<int>(nullable: true),
                    KnownFor = table.Column<string>(nullable: true),
                    Popularity = table.Column<double>(nullable: true),
                    WikipediaUrl = table.Column<string>(maxLength: 200, nullable: true),
                    WordCount = table.Column<int>(nullable: true),
                    DeathWordCount = table.Column<int>(nullable: true),
                    IsStub = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BasePerson", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BasePerson");
        }
    }
}
