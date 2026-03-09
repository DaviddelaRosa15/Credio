using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Credio.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RefactorAmortizationScheduleDates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastPaymentDate",
                table: "AmortizationSchedule");

            migrationBuilder.AddColumn<DateOnly>(
                name: "LastPaymentDate",
                table: "AmortizationSchedule",
                type: "date",
                nullable: true);

            migrationBuilder.DropColumn(
                name: "DueDate",
                table: "AmortizationSchedule");

            migrationBuilder.AddColumn<DateOnly>(
                name: "DueDate",
                table: "AmortizationSchedule",
                type: "date",
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "LastPaymentDate",
                table: "AmortizationSchedule",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DueDate",
                table: "AmortizationSchedule",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");
        }
    }
}
