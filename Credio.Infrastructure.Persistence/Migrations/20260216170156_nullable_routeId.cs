using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Credio.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class nullable_routeId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Client_Route_RouteId",
                table: "Client");

            migrationBuilder.AlterColumn<string>(
                name: "RouteId",
                table: "Client",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddForeignKey(
                name: "FK_Client_Route_RouteId",
                table: "Client",
                column: "RouteId",
                principalTable: "Route",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Client_Route_RouteId",
                table: "Client");

            migrationBuilder.AlterColumn<string>(
                name: "RouteId",
                table: "Client",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Client_Route_RouteId",
                table: "Client",
                column: "RouteId",
                principalTable: "Route",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
