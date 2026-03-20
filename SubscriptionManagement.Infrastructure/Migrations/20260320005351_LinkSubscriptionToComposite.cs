using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class LinkSubscriptionToComposite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_SubscriptionPlans_PlanId",
                table: "Subscriptions");

            migrationBuilder.RenameColumn(
                name: "PlanId",
                table: "Subscriptions",
                newName: "AppId");

            migrationBuilder.RenameIndex(
                name: "IX_Subscriptions_PlanId",
                table: "Subscriptions",
                newName: "IX_Subscriptions_AppId");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_CreativeApps_AppId",
                table: "Subscriptions",
                column: "AppId",
                principalTable: "CreativeApps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_CreativeApps_AppId",
                table: "Subscriptions");

            migrationBuilder.RenameColumn(
                name: "AppId",
                table: "Subscriptions",
                newName: "PlanId");

            migrationBuilder.RenameIndex(
                name: "IX_Subscriptions_AppId",
                table: "Subscriptions",
                newName: "IX_Subscriptions_PlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_SubscriptionPlans_PlanId",
                table: "Subscriptions",
                column: "PlanId",
                principalTable: "SubscriptionPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
