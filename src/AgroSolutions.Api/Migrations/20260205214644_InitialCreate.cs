using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgroSolutions.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Alerts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    FieldId = table.Column<Guid>(type: "TEXT", nullable: false),
                    FarmId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Message = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alerts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SensorReadings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    FieldId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SensorType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Value = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    Unit = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    ReadingTimestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Location = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Metadata = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensorReadings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Role = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Farms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Property_Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Property_Location = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Property_Area = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Property_Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    OwnerName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    OwnerEmail = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    OwnerPhone = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Farms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Farms_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Fields",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    FarmId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Property_Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Property_Location = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Property_Area = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Property_Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    CropType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PlantingDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    HarvestDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Fields_Farms_FarmId",
                        column: x => x.FarmId,
                        principalTable: "Farms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_CreatedAt",
                table: "Alerts",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_FarmId",
                table: "Alerts",
                column: "FarmId");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_FarmId_IsActive",
                table: "Alerts",
                columns: new[] { "FarmId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_FieldId",
                table: "Alerts",
                column: "FieldId");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_FieldId_IsActive",
                table: "Alerts",
                columns: new[] { "FieldId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_IsActive",
                table: "Alerts",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_Status",
                table: "Alerts",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Farms_UserId",
                table: "Farms",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Fields_FarmId",
                table: "Fields",
                column: "FarmId");

            migrationBuilder.CreateIndex(
                name: "IX_SensorReadings_FieldId",
                table: "SensorReadings",
                column: "FieldId");

            migrationBuilder.CreateIndex(
                name: "IX_SensorReadings_FieldId_SensorType_ReadingTimestamp",
                table: "SensorReadings",
                columns: new[] { "FieldId", "SensorType", "ReadingTimestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_SensorReadings_ReadingTimestamp",
                table: "SensorReadings",
                column: "ReadingTimestamp");

            migrationBuilder.CreateIndex(
                name: "IX_SensorReadings_SensorType",
                table: "SensorReadings",
                column: "SensorType");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Role",
                table: "Users",
                column: "Role");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alerts");

            migrationBuilder.DropTable(
                name: "Fields");

            migrationBuilder.DropTable(
                name: "SensorReadings");

            migrationBuilder.DropTable(
                name: "Farms");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
