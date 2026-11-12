using LabEquipmentApi.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LabEquipmentApi.Migrations;

[DbContext(typeof(LabDbContext))]
[Migration("20261112000000_InitialCreate")]
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "EquipmentItems",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "text", nullable: false),
                Room = table.Column<string>(type: "text", nullable: false),
                IsAvailable = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_EquipmentItems", x => x.Id);
            });

        migrationBuilder.InsertData(
            table: "EquipmentItems",
            columns: new[] { "Id", "Name", "Room", "IsAvailable" },
            values: new object[,]
            {
                { 1, "Projector", "Lab 10", true },
                { 2, "Laptop", "Lab 10", false },
                { 3, "Monitor", "Lab 11", true }
            });

        // HasData inserts explicit keys; move the PostgreSQL identity sequence past the seed IDs.
        migrationBuilder.Sql(
            "SELECT setval(pg_get_serial_sequence('\"EquipmentItems\"', 'Id'), (SELECT MAX(\"Id\") FROM \"EquipmentItems\"), true);");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "EquipmentItems");
    }
}
