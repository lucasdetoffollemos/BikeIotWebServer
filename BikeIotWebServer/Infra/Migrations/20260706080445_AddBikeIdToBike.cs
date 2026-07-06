using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BikeIotWebServer.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddBikeIdToBike : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BikeId",
                table: "Bikes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BikeId",
                table: "Bikes");
        }
    }
}
