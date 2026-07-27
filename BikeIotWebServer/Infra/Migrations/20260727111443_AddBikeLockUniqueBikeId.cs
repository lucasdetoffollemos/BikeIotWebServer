using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BikeIotWebServer.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddBikeLockUniqueBikeId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_BikeLocks_BikeId",
                table: "BikeLocks",
                column: "BikeId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BikeLocks_BikeId",
                table: "BikeLocks");
        }
    }
}
