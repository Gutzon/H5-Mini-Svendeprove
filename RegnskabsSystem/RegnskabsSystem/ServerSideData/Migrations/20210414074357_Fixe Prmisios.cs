using Microsoft.EntityFrameworkCore.Migrations;

namespace ServerSideData.Migrations
{
    public partial class FixePrmisios : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AddCorporation",
                table: "Permissions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AddFinance",
                table: "Permissions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AddInventory",
                table: "Permissions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AddMember",
                table: "Permissions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AddUser",
                table: "Permissions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Admin",
                table: "Permissions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "DeleteInventory",
                table: "Permissions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "DeleteMember",
                table: "Permissions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "DeleteUser",
                table: "Permissions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EditInventory",
                table: "Permissions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EditMember",
                table: "Permissions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EditUser",
                table: "Permissions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "LimitedViewFinance",
                table: "Permissions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ViewFinance",
                table: "Permissions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddCorporation",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "AddFinance",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "AddInventory",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "AddMember",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "AddUser",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "Admin",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "DeleteInventory",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "DeleteMember",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "DeleteUser",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "EditInventory",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "EditMember",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "EditUser",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "LimitedViewFinance",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "ViewFinance",
                table: "Permissions");
        }
    }
}
