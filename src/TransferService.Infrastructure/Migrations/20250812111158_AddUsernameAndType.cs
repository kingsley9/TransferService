using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TransferService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUsernameAndType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Accounts",
                type: "int",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "Accounts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: ""
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Type", table: "Accounts");

            migrationBuilder.DropColumn(name: "Username", table: "Accounts");
        }
    }
}
