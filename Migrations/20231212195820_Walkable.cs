using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevameetCSharp.Migrations
{
    public partial class Walkable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Walkable",
                table: "MeetObjects",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Walkable",
                table: "MeetObjects");
        }
    }
}
