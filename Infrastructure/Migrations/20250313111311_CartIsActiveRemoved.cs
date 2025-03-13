using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CartIsActiveRemoved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carts_Users_UserId",
                schema: "Cart",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "Cart",
                table: "Carts");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_Users_UserId",
                schema: "Cart",
                table: "Carts",
                column: "UserId",
                principalSchema: "Security",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carts_Users_UserId",
                schema: "Cart",
                table: "Carts");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "Cart",
                table: "Carts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_Users_UserId",
                schema: "Cart",
                table: "Carts",
                column: "UserId",
                principalSchema: "Security",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
