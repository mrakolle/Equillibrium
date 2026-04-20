using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Equillibrium.QC.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialQCSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "tenants");

            migrationBuilder.CreateTable(
                name: "Specifications",
                schema: "tenants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RecipeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Parameter = table.Column<string>(type: "text", nullable: false),
                    TestMethod = table.Column<string>(type: "text", nullable: false),
                    MinValue = table.Column<decimal>(type: "numeric", nullable: false),
                    TargetValue = table.Column<decimal>(type: "numeric", nullable: false),
                    MaxValue = table.Column<decimal>(type: "numeric", nullable: false),
                    Unit = table.Column<string>(type: "text", nullable: false),
                    RouteStage = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Specifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BatchQcResults",
                schema: "tenants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BatchRecordId = table.Column<Guid>(type: "uuid", nullable: false),
                    SpecificationId = table.Column<Guid>(type: "uuid", nullable: false),
                    MeasuredValue = table.Column<decimal>(type: "numeric", nullable: false),
                    TestedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TestedBy = table.Column<string>(type: "text", nullable: false),
                    IsInTolerance = table.Column<bool>(type: "boolean", nullable: false),
                    Remarks = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatchQcResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BatchQcResults_Specifications_SpecificationId",
                        column: x => x.SpecificationId,
                        principalSchema: "tenants",
                        principalTable: "Specifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BatchQcResults_SpecificationId",
                schema: "tenants",
                table: "BatchQcResults",
                column: "SpecificationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BatchQcResults",
                schema: "tenants");

            migrationBuilder.DropTable(
                name: "Specifications",
                schema: "tenants");
        }
    }
}
