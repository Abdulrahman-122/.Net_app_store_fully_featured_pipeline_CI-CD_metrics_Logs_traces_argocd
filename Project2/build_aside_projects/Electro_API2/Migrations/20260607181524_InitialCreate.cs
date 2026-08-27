using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElectroAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(300)", unicode: false, maxLength: 300, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MainCategory = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Categori__19093A0BB8F1CE42", x => x.CategoryId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    FullName = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PhoneNumber = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ShippingAddress = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PasswordHash = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RegistrationDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsAdmin = table.Column<bool>(type: "tinyint(1)", nullable: true, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Customer__A4AE64D8F6C754CC", x => x.CustomerId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    PaymentId = table.Column<int>(type: "int", nullable: false),
                    PaymentDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Amount = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Method = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Payment__9B556A3820C82027", x => x.PaymentId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Suppliers",
                columns: table => new
                {
                    SupplierId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Country = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Address = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ContactNumber = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Supplier__4BE666B4C123AA4B", x => x.SupplierId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(300)", unicode: false, maxLength: 300, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StockQuantity = table.Column<int>(type: "int", nullable: true),
                    AddedDate = table.Column<DateOnly>(type: "date", nullable: true),
                    UnitPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    ImageUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Products__B40CC6CD857CD8AC", x => x.ProductId);
                    table.ForeignKey(
                        name: "FK__Products__Catego__619B8048",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TotalAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    OrderDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CustomerId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Orders__C3905BCF1020B2F9", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK__Orders__Customer__5165187F",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Cart",
                columns: table => new
                {
                    CartId = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: true),
                    AddedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Cart__51BCD7B735C06D58", x => x.CartId);
                    table.ForeignKey(
                        name: "FK__Cart__CustomerId__160F4887",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId");
                    table.ForeignKey(
                        name: "FK__Cart__ProductId__17036CC0",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Products_Suppliers",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    SupplierId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Products__E0B2A0A666D9A993", x => new { x.ProductId, x.SupplierId });
                    table.ForeignKey(
                        name: "FK__Products___Produ__656C112C",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId");
                    table.ForeignKey(
                        name: "FK__Products___Suppl__6477ECF3",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "SupplierId");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    ReviewId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: true),
                    Comment = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Rating = table.Column<int>(type: "int", nullable: true),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    ProductId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Reviews__74BC79CE9DB88981", x => x.ReviewId);
                    table.ForeignKey(
                        name: "FK__Reviews__Custome__693CA210",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId");
                    table.ForeignKey(
                        name: "FK__Reviews__Product__6A30C649",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "StockTransactions",
                columns: table => new
                {
                    TranId = table.Column<int>(type: "int", nullable: false),
                    TranDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Type = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    QuantityChange = table.Column<int>(type: "int", nullable: true),
                    Reference = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProductId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__StockTra__F70897C98146DCAA", x => x.TranId);
                    table.ForeignKey(
                        name: "FK__StockTran__Produ__70DDC3D8",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    OrderItemId = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: true),
                    OrderId = table.Column<int>(type: "int", nullable: true),
                    ProductId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__OrderIte__57ED0681C67686A8", x => x.OrderItemId);
                    table.ForeignKey(
                        name: "FK__OrderItem__Order__6D0D32F4",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId");
                    table.ForeignKey(
                        name: "FK__OrderItem__Produ__6E01572D",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Orders_Payments",
                columns: table => new
                {
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    PaymentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Orders_P__5A250D6C4F896B9A", x => new { x.OrderId, x.PaymentId });
                    table.ForeignKey(
                        name: "FK__Orders_Pa__Order__5AEE82B9",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId");
                    table.ForeignKey(
                        name: "FK__Orders_Pa__Payme__5BE2A6F2",
                        column: x => x.PaymentId,
                        principalTable: "Payment",
                        principalColumn: "PaymentId");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Shipments",
                columns: table => new
                {
                    ShipmentId = table.Column<int>(type: "int", nullable: false),
                    DeliveryDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ShipmentDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Amount = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CurrierName = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TrackingNumber = table.Column<int>(type: "int", nullable: true),
                    OrderId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Shipment__5CAD37ED2ED43939", x => x.ShipmentId);
                    table.ForeignKey(
                        name: "FK__Shipments__Order__5812160E",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Cart_CustomerId",
                table: "Cart",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Cart_ProductId",
                table: "Cart",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductId",
                table: "OrderItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Payments_PaymentId",
                table: "Orders_Payments",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Suppliers_SupplierId",
                table: "Products_Suppliers",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_CustomerId",
                table: "Reviews",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ProductId",
                table: "Reviews",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_OrderId",
                table: "Shipments",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransactions_ProductId",
                table: "StockTransactions",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cart");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "Orders_Payments");

            migrationBuilder.DropTable(
                name: "Products_Suppliers");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "Shipments");

            migrationBuilder.DropTable(
                name: "StockTransactions");

            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropTable(
                name: "Suppliers");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
