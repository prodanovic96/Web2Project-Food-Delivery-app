using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Web2Project.Migrations
{
    public partial class novovreme : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VremeIsporuke",
                table: "Korpa");

            migrationBuilder.AddColumn<DateTime>(
                name: "VremePorucivanja",
                table: "Korpa",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VremePorucivanja",
                table: "Korpa");

            migrationBuilder.AddColumn<DateTime>(
                name: "VremeIsporuke",
                table: "Korpa",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
