using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IMS.Migrations
{
    public partial class a : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Warehouse_Branch_BranchId",
                table: "Warehouse");

            migrationBuilder.DropIndex(
                name: "IX_Warehouse_BranchId",
                table: "Warehouse");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Warehouse_BranchId",
                table: "Warehouse",
                column: "BranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_Warehouse_Branch_BranchId",
                table: "Warehouse",
                column: "BranchId",
                principalTable: "Branch",
                principalColumn: "BranchId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
