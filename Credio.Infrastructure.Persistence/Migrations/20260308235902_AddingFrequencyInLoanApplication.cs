using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Credio.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddingFrequencyInLoanApplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentFrequencyId",
                table: "LoanApplication",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_LoanApplication_PaymentFrequencyId",
                table: "LoanApplication",
                column: "PaymentFrequencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_LoanApplication_PaymentFrequency_PaymentFrequencyId",
                table: "LoanApplication",
                column: "PaymentFrequencyId",
                principalTable: "PaymentFrequency",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoanApplication_PaymentFrequency_PaymentFrequencyId",
                table: "LoanApplication");

            migrationBuilder.DropIndex(
                name: "IX_LoanApplication_PaymentFrequencyId",
                table: "LoanApplication");

            migrationBuilder.DropColumn(
                name: "PaymentFrequencyId",
                table: "LoanApplication");
        }
    }
}
