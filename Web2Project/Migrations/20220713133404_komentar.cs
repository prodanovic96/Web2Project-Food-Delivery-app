using Microsoft.EntityFrameworkCore.Migrations;

namespace Web2Project.Migrations
{
    public partial class komentar : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Komentar",
                table: "Korpa",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Komentar",
                table: "Korpa");
        }
    }
}
