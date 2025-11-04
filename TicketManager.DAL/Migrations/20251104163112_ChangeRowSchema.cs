using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketManager.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ChangeRowSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RowNumber",
                table: "Rows",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowNumber",
                table: "Rows");
        }
    }
}
