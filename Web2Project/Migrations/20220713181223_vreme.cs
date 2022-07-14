using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Web2Project.Migrations
{
    public partial class vreme : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "VremeIsporuke",
                table: "Korpa",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VremeIsporuke",
                table: "Korpa");
        }
    }
}
