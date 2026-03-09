using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Credio.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RefactorLoanTermAndDates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TermMonths",
                table: "Loan",
                newName: "Term");

            migrationBuilder.DropColumn(
                name: "FirstPaymentDate",
                table: "Loan");

            migrationBuilder.AddColumn<DateOnly>(
                name: "FirstPaymentDate",
                table: "Loan",
                type: "date");

            migrationBuilder.DropColumn(
                name: "DisbursedDate",
                table: "Loan");

            migrationBuilder.AddColumn<DateOnly>(
                name: "DisbursedDate",
                table: "Loan",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "MaturityDate",
                table: "Loan",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaturityDate",
                table: "Loan");

            migrationBuilder.RenameColumn(
                name: "Term",
                table: "Loan",
                newName: "TermMonths");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FirstPaymentDate",
                table: "Loan",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DisbursedDate",
                table: "Loan",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);
        }
    }
}
