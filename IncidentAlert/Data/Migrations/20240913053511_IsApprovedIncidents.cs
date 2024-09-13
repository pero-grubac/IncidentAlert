using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IncidentAlert.Data.Migrations
{
    /// <inheritdoc />
    public partial class IsApprovedIncidents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "incident",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "incident");
        }
    }
}
