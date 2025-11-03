using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketManager.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAppDbContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Carts_CartId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_CartId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Carts_PaymentId",
                table: "Carts");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_PaymentId",
                table: "Carts",
                column: "PaymentId",
                unique: true,
                filter: "[PaymentId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Carts_PaymentId",
                table: "Carts");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_CartId",
                table: "Payments",
                column: "CartId");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_PaymentId",
                table: "Carts",
                column: "PaymentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Carts_CartId",
                table: "Payments",
                column: "CartId",
                principalTable: "Carts",
                principalColumn: "CartId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
