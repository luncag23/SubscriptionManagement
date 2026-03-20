using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddCreativeAppsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CreativeApps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BasePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ParentAppId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreativeApps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreativeApps_CreativeApps_ParentAppId",
                        column: x => x.ParentAppId,
                        principalTable: "CreativeApps",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CreativeApps_ParentAppId",
                table: "CreativeApps",
                column: "ParentAppId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CreativeApps");
        }
    }
}
