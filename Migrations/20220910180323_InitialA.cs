using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IMS.Migrations
{
    public partial class InitialA : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrder_Bill_BillId1",
                table: "PurchaseOrder");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseOrder_BillId1",
                table: "PurchaseOrder");

            migrationBuilder.DropColumn(
                name: "BillId1",
                table: "PurchaseOrder");

            migrationBuilder.AlterColumn<int>(
                name: "BillId",
                table: "PurchaseOrder",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrder_BillId",
                table: "PurchaseOrder",
                column: "BillId");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrder_Bill_BillId",
                table: "PurchaseOrder",
                column: "BillId",
                principalTable: "Bill",
                principalColumn: "BillId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrder_Bill_BillId",
                table: "PurchaseOrder");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseOrder_BillId",
                table: "PurchaseOrder");

            migrationBuilder.AlterColumn<string>(
                name: "BillId",
                table: "PurchaseOrder",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BillId1",
                table: "PurchaseOrder",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrder_BillId1",
                table: "PurchaseOrder",
                column: "BillId1");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrder_Bill_BillId1",
                table: "PurchaseOrder",
                column: "BillId1",
                principalTable: "Bill",
                principalColumn: "BillId");
        }
    }
}
