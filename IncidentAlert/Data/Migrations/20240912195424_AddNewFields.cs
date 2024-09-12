using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IncidentAlert.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNewFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_incident_category_category_CategoryId",
                table: "incident_category");

            migrationBuilder.DropForeignKey(
                name: "FK_incident_category_incident_IncidentId",
                table: "incident_category");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateTime",
                table: "incident",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_category_Name",
                table: "category",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_incident_category_category_CategoryId",
                table: "incident_category",
                column: "CategoryId",
                principalTable: "category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_incident_category_incident_IncidentId",
                table: "incident_category",
                column: "IncidentId",
                principalTable: "incident",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_incident_category_category_CategoryId",
                table: "incident_category");

            migrationBuilder.DropForeignKey(
                name: "FK_incident_category_incident_IncidentId",
                table: "incident_category");

            migrationBuilder.DropIndex(
                name: "IX_category_Name",
                table: "category");

            migrationBuilder.DropColumn(
                name: "DateTime",
                table: "incident");

            migrationBuilder.AddForeignKey(
                name: "FK_incident_category_category_CategoryId",
                table: "incident_category",
                column: "CategoryId",
                principalTable: "category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_incident_category_incident_IncidentId",
                table: "incident_category",
                column: "IncidentId",
                principalTable: "incident",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
