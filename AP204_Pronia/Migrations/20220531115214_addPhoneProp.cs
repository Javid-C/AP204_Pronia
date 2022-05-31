using Microsoft.EntityFrameworkCore.Migrations;

namespace AP204_Pronia.Migrations
{
    public partial class addPhoneProp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Settings",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Settings");
        }
    }
}
