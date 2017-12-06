namespace ETicketSystem.Data.Migrations
{
	using Microsoft.EntityFrameworkCore.Migrations;

	public partial class AddRouteAcitveStateAndDuration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ArrivalTime",
                table: "Routes",
                newName: "Duration");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Routes",
                nullable: false,
                defaultValue: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Routes");

            migrationBuilder.RenameColumn(
                name: "Duration",
                table: "Routes",
                newName: "ArrivalTime");
        }
    }
}
