using System;

using Microsoft.EntityFrameworkCore.Migrations;

using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Airport.Api.Migrations
{
    /// <inheritdoc />
    public partial class Add_Gate_Event : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GateEventTypes",
                columns: table => new
                {
                    gateEventTypeId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GateEventTypes", x => x.gateEventTypeId);
                });

            migrationBuilder.CreateTable(
                name: "GateEvents",
                columns: table => new
                {
                    gateEventId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StartDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    gateId = table.Column<int>(type: "integer", nullable: false),
                    GateEventTypeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GateEvents", x => x.gateEventId);
                    table.ForeignKey(
                        name: "FK_GateEvents_GateEventTypes_GateEventTypeId",
                        column: x => x.GateEventTypeId,
                        principalTable: "GateEventTypes",
                        principalColumn: "gateEventTypeId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_GateEvents_Gates_gateId",
                        column: x => x.gateId,
                        principalTable: "Gates",
                        principalColumn: "gateId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "GateEventTypes",
                columns: new[] { "gateEventTypeId", "description" },
                values: new object[,]
                {
                    { 1, "Departure" },
                    { 2, "Arrival" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_GateEvents_GateEventTypeId",
                table: "GateEvents",
                column: "GateEventTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_GateEvents_gateId",
                table: "GateEvents",
                column: "gateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GateEvents");

            migrationBuilder.DropTable(
                name: "GateEventTypes");
        }
    }
}
