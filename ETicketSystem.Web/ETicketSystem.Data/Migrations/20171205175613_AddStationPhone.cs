namespace ETicketSystem.Data.Migrations
{
	using Microsoft.EntityFrameworkCore.Migrations;

	public partial class AddStationPhone : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Stations",
                maxLength: 15,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Stations");
        }
    }
}
