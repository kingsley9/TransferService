using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TransferService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerAndValidations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OwnerName",
                table: "Accounts",
                newName: "SchemeCode"
            );

            migrationBuilder.AddColumn<string>(
                name: "AccountName",
                table: "Accounts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: ""
            );

            migrationBuilder.AddColumn<string>(
                name: "AccountNumber",
                table: "Accounts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: ""
            );

            migrationBuilder.AddColumn<string>(
                name: "BankCode",
                table: "Accounts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: ""
            );

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Accounts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: ""
            );

            migrationBuilder.AddColumn<Guid>(
                name: "CustomerId",
                table: "Accounts",
                type: "uniqueidentifier",
                nullable: true,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000")
            );

            migrationBuilder.AddColumn<bool>(
                name: "IsPND",
                table: "Accounts",
                type: "bit",
                nullable: false,
                defaultValue: false
            );

            migrationBuilder.AddColumn<decimal>(
                name: "LienAmount",
                table: "Accounts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m
            );

            migrationBuilder.AddColumn<string>(
                name: "PinHash",
                table: "Accounts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: ""
            );

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Accounts",
                type: "int",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder.AddColumn<int>(
                name: "Tier",
                table: "Accounts",
                type: "int",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder.AddColumn<decimal>(
                name: "TotalLodgements",
                table: "Accounts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m
            );

            migrationBuilder.AddColumn<decimal>(
                name: "TotalWithdrawals",
                table: "Accounts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m
            );

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_CustomerId",
                table: "Accounts",
                column: "CustomerId"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Customers_CustomerId",
                table: "Accounts",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Customers_CustomerId",
                table: "Accounts"
            );

            migrationBuilder.DropTable(name: "Customers");

            migrationBuilder.DropIndex(name: "IX_Accounts_CustomerId", table: "Accounts");

            migrationBuilder.DropColumn(name: "AccountName", table: "Accounts");

            migrationBuilder.DropColumn(name: "AccountNumber", table: "Accounts");

            migrationBuilder.DropColumn(name: "BankCode", table: "Accounts");

            migrationBuilder.DropColumn(name: "Currency", table: "Accounts");

            migrationBuilder.DropColumn(name: "CustomerId", table: "Accounts");

            migrationBuilder.DropColumn(name: "IsPND", table: "Accounts");

            migrationBuilder.DropColumn(name: "LienAmount", table: "Accounts");

            migrationBuilder.DropColumn(name: "PinHash", table: "Accounts");

            migrationBuilder.DropColumn(name: "Status", table: "Accounts");

            migrationBuilder.DropColumn(name: "Tier", table: "Accounts");

            migrationBuilder.DropColumn(name: "TotalLodgements", table: "Accounts");

            migrationBuilder.DropColumn(name: "TotalWithdrawals", table: "Accounts");

            migrationBuilder.RenameColumn(
                name: "SchemeCode",
                table: "Accounts",
                newName: "OwnerName"
            );
        }
    }
}
