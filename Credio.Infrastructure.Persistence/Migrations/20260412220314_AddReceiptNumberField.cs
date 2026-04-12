using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Credio.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddReceiptNumberField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReceiptNumber",
                table: "Payment",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceiptNumber",
                table: "Payment");
        }
    }
}
