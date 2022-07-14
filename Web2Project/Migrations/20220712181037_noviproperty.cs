using Microsoft.EntityFrameworkCore.Migrations;

namespace Web2Project.Migrations
{
    public partial class noviproperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Broj",
                table: "Korpa");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Korpa",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Korpa");

            migrationBuilder.AddColumn<int>(
                name: "Broj",
                table: "Korpa",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
