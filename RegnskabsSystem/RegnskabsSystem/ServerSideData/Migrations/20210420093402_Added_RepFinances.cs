using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ServerSideData.Migrations
{
    public partial class Added_RepFinances : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RepFinanceEntries",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KontiID = table.Column<int>(type: "int", nullable: false),
                    value = table.Column<double>(type: "float", nullable: false),
                    comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    byWho = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    intervalType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    intervalValue = table.Column<int>(type: "int", nullable: false),
                    firstExecDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    nextExecDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RepFinanceEntries", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RepFinanceEntries");
        }
    }
}
