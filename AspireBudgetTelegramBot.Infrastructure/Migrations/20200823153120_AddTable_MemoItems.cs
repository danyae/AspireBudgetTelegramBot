using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AspireBudgetTelegramBot.Infrastructure.Migrations
{
    public partial class AddTable_MemoItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "memo_item",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    memo = table.Column<string>(nullable: false),
                    type = table.Column<string>(nullable: false),
                    category = table.Column<string>(nullable: true),
                    account_from = table.Column<string>(nullable: false),
                    account_to = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_memo_item", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_memo_item_memo",
                table: "memo_item",
                column: "memo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "memo_item");
        }
    }
}
