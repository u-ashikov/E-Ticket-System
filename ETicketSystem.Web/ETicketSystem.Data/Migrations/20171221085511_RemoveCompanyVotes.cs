namespace ETicketSystem.Data.Migrations
{
	using Microsoft.EntityFrameworkCore.Migrations;

	public partial class RemoveCompanyVotes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DownVotes",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UpVotes",
                table: "AspNetUsers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DownVotes",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpVotes",
                table: "AspNetUsers",
                nullable: true);
        }
    }
}
