using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BikeIotWebServer.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddBikeLock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BikeLocks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BikeId = table.Column<int>(type: "integer", nullable: false),
                    IsLock = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BikeLocks", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BikeLocks");
        }
    }
}
