using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IncidentAlert.Data.Migrations
{
    /// <inheritdoc />
    public partial class IsApprovedRemovedTitleAddedToIncidents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "incident");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "incident",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "incident");

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "incident",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
