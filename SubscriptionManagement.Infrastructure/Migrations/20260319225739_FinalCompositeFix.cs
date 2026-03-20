using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class FinalCompositeFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CreativeApps_CreativeApps_ParentAppId",
                table: "CreativeApps");

            migrationBuilder.DropIndex(
                name: "IX_CreativeApps_ParentAppId",
                table: "CreativeApps");

            migrationBuilder.DropColumn(
                name: "ParentAppId",
                table: "CreativeApps");

            migrationBuilder.CreateTable(
                name: "AppBundleAssignments",
                columns: table => new
                {
                    BundleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AppId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppBundleAssignments", x => new { x.BundleId, x.AppId });
                    table.ForeignKey(
                        name: "FK_AppBundleAssignments_CreativeApps_AppId",
                        column: x => x.AppId,
                        principalTable: "CreativeApps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppBundleAssignments_CreativeApps_BundleId",
                        column: x => x.BundleId,
                        principalTable: "CreativeApps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppBundleAssignments_AppId",
                table: "AppBundleAssignments",
                column: "AppId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppBundleAssignments");

            migrationBuilder.AddColumn<Guid>(
                name: "ParentAppId",
                table: "CreativeApps",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CreativeApps_ParentAppId",
                table: "CreativeApps",
                column: "ParentAppId");

            migrationBuilder.AddForeignKey(
                name: "FK_CreativeApps_CreativeApps_ParentAppId",
                table: "CreativeApps",
                column: "ParentAppId",
                principalTable: "CreativeApps",
                principalColumn: "Id");
        }
    }
}
