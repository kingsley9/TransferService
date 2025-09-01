using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TransferService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ModifySchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "FromAccountId", table: "Transactions");

            migrationBuilder.RenameColumn(
                name: "ToAccountId",
                table: "Transactions",
                newName: "TargetAccountId"
            );

            migrationBuilder.AddColumn<int>(
                name: "AccountId",
                table: "Transactions",
                type: "int",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Transactions",
                type: "int",
                nullable: false,
                defaultValue: 0
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "AccountId", table: "Transactions");

            migrationBuilder.DropColumn(name: "Status", table: "Transactions");

            migrationBuilder.RenameColumn(
                name: "TargetAccountId",
                table: "Transactions",
                newName: "ToAccountId"
            );

            migrationBuilder.AddColumn<int>(
                name: "FromAccountId",
                table: "Transactions",
                type: "int",
                nullable: true
            );
        }
    }
}
