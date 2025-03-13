using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DbConfigChagedForOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carts_Users_UserId",
                schema: "Cart",
                table: "Carts");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Addresses_BillingAddressId",
                schema: "Order",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Addresses_ShippingAddressId",
                schema: "Order",
                table: "Orders");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_Users_UserId",
                schema: "Cart",
                table: "Carts",
                column: "UserId",
                principalSchema: "Security",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Addresses_BillingAddressId",
                schema: "Order",
                table: "Orders",
                column: "BillingAddressId",
                principalSchema: "User",
                principalTable: "Addresses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Addresses_ShippingAddressId",
                schema: "Order",
                table: "Orders",
                column: "ShippingAddressId",
                principalSchema: "User",
                principalTable: "Addresses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carts_Users_UserId",
                schema: "Cart",
                table: "Carts");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Addresses_BillingAddressId",
                schema: "Order",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Addresses_ShippingAddressId",
                schema: "Order",
                table: "Orders");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_Users_UserId",
                schema: "Cart",
                table: "Carts",
                column: "UserId",
                principalSchema: "Security",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Addresses_BillingAddressId",
                schema: "Order",
                table: "Orders",
                column: "BillingAddressId",
                principalSchema: "User",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Addresses_ShippingAddressId",
                schema: "Order",
                table: "Orders",
                column: "ShippingAddressId",
                principalSchema: "User",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
