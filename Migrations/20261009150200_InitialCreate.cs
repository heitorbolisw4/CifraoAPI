using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExchangeRates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BaseCurrency = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false),
                    QuoteCurrency = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false),
                    RateDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Bid = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    Ask = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    Source = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    FetchedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeRates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    IsSystem = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Categories_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Expenses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    Date = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateOnly>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expenses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Expenses_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Expenses_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "IsSystem", "Name", "UserId" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 9, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Alimentacao", null },
                    { 2, new DateTime(2026, 9, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Transporte", null },
                    { 3, new DateTime(2026, 9, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Moradia", null },
                    { 4, new DateTime(2026, 9, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Saude", null },
                    { 5, new DateTime(2026, 9, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Educacao", null },
                    { 6, new DateTime(2026, 9, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Lazer", null },
                    { 7, new DateTime(2026, 9, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Outros", null },
                    { 11, new DateTime(2026, 9, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Outros", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_UserId_Name",
                table: "Categories",
                columns: new[] { "UserId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeRates_BaseCurrency_QuoteCurrency_RateDate",
                table: "ExchangeRates",
                columns: new[] { "BaseCurrency", "QuoteCurrency", "RateDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_CategoryId",
                table: "Expenses",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_UserId",
                table: "Expenses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExchangeRates");

            migrationBuilder.DropTable(
                name: "Expenses");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
