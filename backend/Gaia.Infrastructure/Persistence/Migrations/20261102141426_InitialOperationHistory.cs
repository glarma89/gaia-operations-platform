using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gaia.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialOperationHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OperationHistory",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OperationId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    InputA = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InputB = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResultJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExecutedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationHistory", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OperationHistory_OperationId_ExecutedAtUtc",
                table: "OperationHistory",
                columns: new[] { "OperationId", "ExecutedAtUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OperationHistory");
        }
    }
}
