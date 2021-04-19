using Microsoft.EntityFrameworkCore.Migrations;

namespace ServerSideData.Migrations
{
    public partial class updatedfinacemodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "newSaldoKonti",
                table: "FinanceEntries",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "newSaldoMain",
                table: "FinanceEntries",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "newSaldoKonti",
                table: "FinanceEntries");

            migrationBuilder.DropColumn(
                name: "newSaldoMain",
                table: "FinanceEntries");
        }
    }
}
