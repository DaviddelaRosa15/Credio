using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Credio.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FinalSnapshotFieldsToPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "InterestAmount",
                table: "Payment",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "LateFeeAmount",
                table: "Payment",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PrincipalAmount",
                table: "Payment",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RemainingPrincipal",
                table: "Payment",
                type: "numeric",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InterestAmount",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "LateFeeAmount",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "PrincipalAmount",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "RemainingPrincipal",
                table: "Payment");
        }
    }
}
