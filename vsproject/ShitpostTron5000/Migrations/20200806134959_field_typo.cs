using Microsoft.EntityFrameworkCore.Migrations;

namespace ShitpostTron5000.Migrations
{
    public partial class field_typo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "QouteeDiscordSnowflake",
                table: "Quotes",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.Sql("UPDATE Quotes SET QouteeDiscordSnowflake = QouteeDiscordSnoflake;");

            migrationBuilder.DropColumn(
                name: "QouteeDiscordSnoflake",
                table: "Quotes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "QouteeDiscordSnoflake",
                table: "Quotes",
                type: "decimal(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.Sql("UPDATE Quotes SET QouteeDiscordSnoflake = QouteeDiscordSnowflake;");

            migrationBuilder.DropColumn(
                name: "QouteeDiscordSnowflake",
                table: "Quotes");
        }
    }
}
