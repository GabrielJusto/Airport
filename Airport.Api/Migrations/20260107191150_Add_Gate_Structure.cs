using Microsoft.EntityFrameworkCore.Migrations;

using NetTopologySuite.Geometries;

using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Airport.Api.Migrations
{
    /// <inheritdoc />
    public partial class Add_Gate_Structure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.CreateTable(
                name: "Hubs",
                columns: table => new
                {
                    hubId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Location = table.Column<Point>(type: "geography (point)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hubs", x => x.hubId);
                });

            migrationBuilder.CreateTable(
                name: "Terminals",
                columns: table => new
                {
                    terminalId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    hubId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Terminals", x => x.terminalId);
                    table.ForeignKey(
                        name: "FK_Terminals_Hubs_hubId",
                        column: x => x.hubId,
                        principalTable: "Hubs",
                        principalColumn: "hubId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Gates",
                columns: table => new
                {
                    gateId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    jetways = table.Column<int>(type: "integer", nullable: false),
                    maxPassengersPerHour = table.Column<int>(type: "integer", nullable: false),
                    terminalId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gates", x => x.gateId);
                    table.ForeignKey(
                        name: "FK_Gates_Terminals_terminalId",
                        column: x => x.terminalId,
                        principalTable: "Terminals",
                        principalColumn: "terminalId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Gates_terminalId",
                table: "Gates",
                column: "terminalId");

            migrationBuilder.CreateIndex(
                name: "IX_Terminals_hubId",
                table: "Terminals",
                column: "hubId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Gates");

            migrationBuilder.DropTable(
                name: "Terminals");

            migrationBuilder.DropTable(
                name: "Hubs");
        }
    }
}
