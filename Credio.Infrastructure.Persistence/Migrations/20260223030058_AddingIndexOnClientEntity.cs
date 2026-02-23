using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Credio.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddingIndexOnClientEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Client_FirstName",
                table: "Client",
                column: "FirstName");

            migrationBuilder.CreateIndex(
                name: "IX_Client_FullName",
                table: "Client",
                columns: new[] { "FirstName", "LastName" });

            migrationBuilder.CreateIndex(
                name: "IX_Client_LastName",
                table: "Client",
                column: "LastName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Client_FirstName",
                table: "Client");

            migrationBuilder.DropIndex(
                name: "IX_Client_FullName",
                table: "Client");

            migrationBuilder.DropIndex(
                name: "IX_Client_LastName",
                table: "Client");
        }
    }
}
