using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IncidentAlert.Data.Migrations
{
    /// <inheritdoc />
    public partial class IncidentLocationRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_incident_location_LocationId",
                table: "incident");

            migrationBuilder.AddForeignKey(
                name: "FK_incident_location_LocationId",
                table: "incident",
                column: "LocationId",
                principalTable: "location",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_incident_location_LocationId",
                table: "incident");

            migrationBuilder.AddForeignKey(
                name: "FK_incident_location_LocationId",
                table: "incident",
                column: "LocationId",
                principalTable: "location",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
