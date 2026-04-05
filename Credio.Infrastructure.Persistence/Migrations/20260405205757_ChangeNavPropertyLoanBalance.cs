using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Credio.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChangeNavPropertyLoanBalance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LoanBalance_LoanId",
                table: "LoanBalance");

            migrationBuilder.CreateIndex(
                name: "IX_LoanBalance_LoanId",
                table: "LoanBalance",
                column: "LoanId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LoanBalance_LoanId",
                table: "LoanBalance");

            migrationBuilder.CreateIndex(
                name: "IX_LoanBalance_LoanId",
                table: "LoanBalance",
                column: "LoanId");
        }
    }
}
