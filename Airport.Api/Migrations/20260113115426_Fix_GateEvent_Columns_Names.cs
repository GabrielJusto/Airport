using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Airport.Api.Migrations
{
    /// <inheritdoc />
    public partial class Fix_GateEvent_Columns_Names : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "GateEvents",
                newName: "startDate");

            migrationBuilder.RenameColumn(
                name: "EndDate",
                table: "GateEvents",
                newName: "endDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "startDate",
                table: "GateEvents",
                newName: "StartDate");

            migrationBuilder.RenameColumn(
                name: "endDate",
                table: "GateEvents",
                newName: "EndDate");
        }
    }
}
