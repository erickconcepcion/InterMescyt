using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace InterMescyt.Migrations
{
    public partial class BankRefactor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TransactionBankNumber",
                table: "Executions",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "HeaderBanks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Rnc = table.Column<string>(maxLength: 11, nullable: true),
                    TransDate = table.Column<DateTime>(nullable: false),
                    InputDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeaderBanks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransLineBanks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Cedula = table.Column<string>(maxLength: 11, nullable: true),
                    BankAccount = table.Column<string>(maxLength: 10, nullable: true),
                    NetSalary = table.Column<decimal>(nullable: false),
                    TransDate = table.Column<DateTime>(nullable: false),
                    HeaderBankId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransLineBanks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransLineBanks_HeaderBanks_HeaderBankId",
                        column: x => x.HeaderBankId,
                        principalTable: "HeaderBanks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TransLineBanks_HeaderBankId",
                table: "TransLineBanks",
                column: "HeaderBankId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TransLineBanks");

            migrationBuilder.DropTable(
                name: "HeaderBanks");

            migrationBuilder.DropColumn(
                name: "TransactionBankNumber",
                table: "Executions");
        }
    }
}
