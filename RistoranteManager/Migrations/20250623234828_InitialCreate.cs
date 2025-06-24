using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RistoranteManager.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DoughTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdditionalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoughTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Extras",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Extras", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsCustomizable = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tables",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Number = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Covers = table.Column<int>(type: "int", nullable: false),
                    UseCount = table.Column<int>(type: "int", nullable: false),
                    IsClosed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tables", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsSent = table.Column<bool>(type: "bit", nullable: false),
                    IsClosed = table.Column<bool>(type: "bit", nullable: false),
                    TableId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Tables_TableId",
                        column: x => x.TableId,
                        principalTable: "Tables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DoughType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItemExtras",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderItemId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItemExtras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItemExtras_OrderItems_OrderItemId",
                        column: x => x.OrderItemId,
                        principalTable: "OrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "DoughTypes",
                columns: new[] { "Id", "AdditionalPrice", "Name" },
                values: new object[,]
                {
                    { 1, 0m, "Classica" },
                    { 2, 0m, "Napoli" },
                    { 3, 2m, "Cereali" },
                    { 4, 2m, "Senza Glutine" }
                });

            migrationBuilder.InsertData(
                table: "Extras",
                columns: new[] { "Id", "Name", "Price" },
                values: new object[,]
                {
                    { 1, "Mozzarella senza lattosio", 1.5m },
                    { 2, "Bufala", 2m },
                    { 3, "Funghi porcini", 2.5m },
                    { 4, "Prosciutto crudo", 2m }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Category", "IsCustomizable", "Name", "Price", "Type" },
                values: new object[,]
                {
                    { 1, "antipasti", false, "Bruschetta al Pomodoro", 8m, "kitchen" },
                    { 2, "antipasti", false, "Antipasto Misto", 12m, "kitchen" },
                    { 3, "antipasti", false, "Caprese", 10m, "kitchen" },
                    { 4, "antipasti", false, "Frittura di Mare", 12m, "kitchen" },
                    { 5, "pasta", false, "Spaghetti alla Carbonara", 14m, "kitchen" },
                    { 6, "pasta", false, "Penne all'Arrabbiata", 14m, "kitchen" },
                    { 7, "pasta", false, "Tagliatelle ai Funghi Porcini", 16m, "kitchen" },
                    { 8, "pasta", false, "Risotto ai Frutti di Mare", 18m, "kitchen" },
                    { 9, "pizza", true, "Margherita", 9m, "pizzeria" },
                    { 10, "pizza", true, "Diavola", 11m, "pizzeria" },
                    { 11, "pizza", true, "Quattro Formaggi", 12m, "pizzeria" },
                    { 12, "pizza", true, "Capricciosa", 13m, "pizzeria" },
                    { 13, "pizza", true, "Napoletana", 10m, "pizzeria" },
                    { 14, "pizza", true, "Prosciutto e Funghi", 12m, "pizzeria" },
                    { 15, "dessert", false, "Tiramisù", 6m, "kitchen" },
                    { 16, "dessert", false, "Panna Cotta", 6m, "kitchen" },
                    { 17, "dessert", false, "Cannoli Siciliani", 7m, "kitchen" },
                    { 18, "dessert", false, "Gelato Artigianale", 6m, "kitchen" }
                });

            migrationBuilder.InsertData(
                table: "Tables",
                columns: new[] { "Id", "Covers", "IsClosed", "Number", "Status", "UseCount" },
                values: new object[,]
                {
                    { 1, 0, false, 1, 0, 0 },
                    { 2, 0, false, 2, 0, 0 },
                    { 3, 0, false, 3, 0, 0 },
                    { 4, 0, false, 4, 0, 0 },
                    { 5, 0, false, 5, 0, 0 },
                    { 6, 0, false, 6, 0, 0 },
                    { 7, 0, false, 7, 0, 0 },
                    { 8, 0, false, 8, 0, 0 },
                    { 9, 0, false, 9, 0, 0 },
                    { 10, 0, false, 10, 0, 0 },
                    { 11, 0, false, 11, 0, 0 },
                    { 12, 0, false, 12, 0, 0 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemExtras_OrderItemId",
                table: "OrderItemExtras",
                column: "OrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductId",
                table: "OrderItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_TableId",
                table: "Orders",
                column: "TableId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DoughTypes");

            migrationBuilder.DropTable(
                name: "Extras");

            migrationBuilder.DropTable(
                name: "OrderItemExtras");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Tables");
        }
    }
}
