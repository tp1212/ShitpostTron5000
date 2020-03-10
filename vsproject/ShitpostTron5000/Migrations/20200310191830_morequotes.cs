using Microsoft.EntityFrameworkCore.Migrations;

namespace ShitpostTron5000.Migrations
{
    public partial class morequotes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quoted",
                table: "Quotes");

            migrationBuilder.AddColumn<decimal>(
                name: "DiscordSnowFlake",
                table: "Quotes",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "QouteeDiscordSnoflake",
                table: "Quotes",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "QuoteeName",
                table: "Quotes",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscordSnowFlake",
                table: "Quotes");

            migrationBuilder.DropColumn(
                name: "QouteeDiscordSnoflake",
                table: "Quotes");

            migrationBuilder.DropColumn(
                name: "QuoteeName",
                table: "Quotes");

            migrationBuilder.AddColumn<decimal>(
                name: "Quoted",
                table: "Quotes",
                type: "decimal(20,0)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
