using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bus_Buddy.Migrations
{
    /// <inheritdoc />
    public partial class AddActivitiesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Routes",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Routes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Endorsements",
                table: "Drivers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Drivers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "HireDate",
                table: "Drivers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Drivers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LicenseClass",
                table: "Drivers",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LicenseExpiryDate",
                table: "Drivers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LicenseIssueDate",
                table: "Drivers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LicenseNumber",
                table: "Drivers",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Drivers",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Drivers",
                keyColumn: "DriverId",
                keyValue: 1,
                columns: new[] { "Endorsements", "FirstName", "HireDate", "LastName", "LicenseClass", "LicenseExpiryDate", "LicenseIssueDate", "LicenseNumber", "Status" },
                values: new object[] { null, "", null, "", null, null, null, null, "Active" });

            migrationBuilder.UpdateData(
                table: "Drivers",
                keyColumn: "DriverId",
                keyValue: 2,
                columns: new[] { "Endorsements", "FirstName", "HireDate", "LastName", "LicenseClass", "LicenseExpiryDate", "LicenseIssueDate", "LicenseNumber", "Status" },
                values: new object[] { null, "", null, "", null, null, null, null, "Active" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Routes");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Routes");

            migrationBuilder.DropColumn(
                name: "Endorsements",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "HireDate",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "LicenseClass",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "LicenseExpiryDate",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "LicenseIssueDate",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "LicenseNumber",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Drivers");
        }
    }
}
