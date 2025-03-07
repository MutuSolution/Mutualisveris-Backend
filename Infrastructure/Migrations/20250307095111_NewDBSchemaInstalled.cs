using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NewDBSchemaInstalled : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                schema: "Security",
                table: "Users");

            migrationBuilder.AddColumn<int>(
                name: "LikeCount",
                schema: "Catalog",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BillingAddressId",
                schema: "Order",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShippingAddressId",
                schema: "Order",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Level",
                schema: "Catalog",
                table: "Categories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "Cart",
                table: "Carts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitPrice",
                schema: "Cart",
                table: "CartItems",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_BillingAddressId",
                schema: "Order",
                table: "Orders",
                column: "BillingAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ShippingAddressId",
                schema: "Order",
                table: "Orders",
                column: "ShippingAddressId");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Addresses_BillingAddressId",
                schema: "Order",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Addresses_ShippingAddressId",
                schema: "Order",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_BillingAddressId",
                schema: "Order",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_ShippingAddressId",
                schema: "Order",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "LikeCount",
                schema: "Catalog",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "BillingAddressId",
                schema: "Order",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShippingAddressId",
                schema: "Order",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Level",
                schema: "Catalog",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "Cart",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "UnitPrice",
                schema: "Cart",
                table: "CartItems");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                schema: "Security",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
