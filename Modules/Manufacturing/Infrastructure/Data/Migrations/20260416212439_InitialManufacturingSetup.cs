using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Equillibrium.Manufacturing.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialManufacturingSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "tenants");

            migrationBuilder.CreateTable(
                name: "BatchRecords",
                schema: "tenants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BatchNumber = table.Column<string>(type: "text", nullable: false),
                    RecipeId = table.Column<Guid>(type: "uuid", nullable: false),
                    RecipeVersionAtSnapshot = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ScheduledDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ActualStartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ActualEndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsApproved = table.Column<bool>(type: "boolean", nullable: false),
                    ApprovedByUserId = table.Column<string>(type: "text", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ApprovalComments = table.Column<string>(type: "text", nullable: true),
                    PlannedQuantity = table.Column<decimal>(type: "numeric", nullable: false),
                    ActualYield = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatchRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ingredients",
                schema: "tenants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    UnitOfMeasure = table.Column<string>(type: "text", nullable: false),
                    MinimumStockLevel = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ingredients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IngredientSds",
                schema: "tenants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IngredientId = table.Column<Guid>(type: "uuid", nullable: false),
                    HazardStatements = table.Column<string>(type: "text", nullable: false),
                    SignalWord = table.Column<string>(type: "text", nullable: false),
                    DocumentUrl = table.Column<string>(type: "text", nullable: false),
                    LastReviewed = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IngredientSds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Recipes",
                schema: "tenants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ProductCode = table.Column<string>(type: "text", nullable: false),
                    Version = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recipes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BatchDeviations",
                schema: "tenants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BatchRecordId = table.Column<Guid>(type: "uuid", nullable: false),
                    OccurredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    ImpactAssessment = table.Column<string>(type: "text", nullable: false),
                    RequiresManagementSignOff = table.Column<bool>(type: "boolean", nullable: false),
                    Resolution = table.Column<string>(type: "text", nullable: true),
                    LoggedByUserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatchDeviations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BatchDeviations_BatchRecords_BatchRecordId",
                        column: x => x.BatchRecordId,
                        principalSchema: "tenants",
                        principalTable: "BatchRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BatchRouteProgress",
                schema: "tenants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BatchRecordId = table.Column<Guid>(type: "uuid", nullable: false),
                    StepType = table.Column<int>(type: "integer", nullable: false),
                    Sequence = table.Column<int>(type: "integer", nullable: false),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    CompletedByUserId = table.Column<string>(type: "text", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ResultPassed = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatchRouteProgress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BatchRouteProgress_BatchRecords_BatchRecordId",
                        column: x => x.BatchRecordId,
                        principalSchema: "tenants",
                        principalTable: "BatchRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaterialLots",
                schema: "tenants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IngredientId = table.Column<Guid>(type: "uuid", nullable: false),
                    SupplierLotNo = table.Column<string>(type: "text", nullable: false),
                    InternalBatchNo = table.Column<string>(type: "text", nullable: false),
                    QuantityReceived = table.Column<decimal>(type: "numeric", nullable: false),
                    QuantityRemaining = table.Column<decimal>(type: "numeric", nullable: false),
                    CostAtPurchase = table.Column<decimal>(type: "numeric", nullable: false),
                    ReceivedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialLots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaterialLots_Ingredients_IngredientId",
                        column: x => x.IngredientId,
                        principalSchema: "tenants",
                        principalTable: "Ingredients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BOMItems",
                schema: "tenants",
                columns: table => new
                {
                    RecipeId = table.Column<Guid>(type: "uuid", nullable: false),
                    IngredientId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric", nullable: false),
                    Unit = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BOMItems", x => new { x.RecipeId, x.IngredientId });
                    table.ForeignKey(
                        name: "FK_BOMItems_Ingredients_IngredientId",
                        column: x => x.IngredientId,
                        principalSchema: "tenants",
                        principalTable: "Ingredients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BOMItems_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalSchema: "tenants",
                        principalTable: "Recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Instructions",
                schema: "tenants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RecipeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Sequence = table.Column<int>(type: "integer", nullable: false),
                    Activity = table.Column<string>(type: "text", nullable: false),
                    DetailedText = table.Column<string>(type: "text", nullable: false),
                    RelatedBOMItemId = table.Column<Guid>(type: "uuid", nullable: true),
                    TargetValue = table.Column<decimal>(type: "numeric", nullable: true),
                    Unit = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instructions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Instructions_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalSchema: "tenants",
                        principalTable: "Recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RouteSteps",
                schema: "tenants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RecipeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Sequence = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    RequiresSignOff = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RouteSteps_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalSchema: "tenants",
                        principalTable: "Recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BatchMaterialUsages",
                schema: "tenants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BatchRecordId = table.Column<Guid>(type: "uuid", nullable: false),
                    MaterialLotId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuantityUsed = table.Column<decimal>(type: "numeric", nullable: false),
                    LoggedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ScannedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatchMaterialUsages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BatchMaterialUsages_BatchRecords_BatchRecordId",
                        column: x => x.BatchRecordId,
                        principalSchema: "tenants",
                        principalTable: "BatchRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BatchMaterialUsages_MaterialLots_MaterialLotId",
                        column: x => x.MaterialLotId,
                        principalSchema: "tenants",
                        principalTable: "MaterialLots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BatchDeviations_BatchRecordId",
                schema: "tenants",
                table: "BatchDeviations",
                column: "BatchRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_BatchMaterialUsages_BatchRecordId",
                schema: "tenants",
                table: "BatchMaterialUsages",
                column: "BatchRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_BatchMaterialUsages_MaterialLotId",
                schema: "tenants",
                table: "BatchMaterialUsages",
                column: "MaterialLotId");

            migrationBuilder.CreateIndex(
                name: "IX_BatchRouteProgress_BatchRecordId",
                schema: "tenants",
                table: "BatchRouteProgress",
                column: "BatchRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_BOMItems_IngredientId",
                schema: "tenants",
                table: "BOMItems",
                column: "IngredientId");

            migrationBuilder.CreateIndex(
                name: "IX_Instructions_RecipeId",
                schema: "tenants",
                table: "Instructions",
                column: "RecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialLots_IngredientId",
                schema: "tenants",
                table: "MaterialLots",
                column: "IngredientId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialLots_SupplierLotNo",
                schema: "tenants",
                table: "MaterialLots",
                column: "SupplierLotNo");

            migrationBuilder.CreateIndex(
                name: "IX_RouteSteps_RecipeId",
                schema: "tenants",
                table: "RouteSteps",
                column: "RecipeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BatchDeviations",
                schema: "tenants");

            migrationBuilder.DropTable(
                name: "BatchMaterialUsages",
                schema: "tenants");

            migrationBuilder.DropTable(
                name: "BatchRouteProgress",
                schema: "tenants");

            migrationBuilder.DropTable(
                name: "BOMItems",
                schema: "tenants");

            migrationBuilder.DropTable(
                name: "IngredientSds",
                schema: "tenants");

            migrationBuilder.DropTable(
                name: "Instructions",
                schema: "tenants");

            migrationBuilder.DropTable(
                name: "RouteSteps",
                schema: "tenants");

            migrationBuilder.DropTable(
                name: "MaterialLots",
                schema: "tenants");

            migrationBuilder.DropTable(
                name: "BatchRecords",
                schema: "tenants");

            migrationBuilder.DropTable(
                name: "Recipes",
                schema: "tenants");

            migrationBuilder.DropTable(
                name: "Ingredients",
                schema: "tenants");
        }
    }
}
