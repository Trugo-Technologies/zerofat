using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.PostgreSQL.Migrations.ClientPortal;

/// <inheritdoc />
public partial class publishDiscountRule : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<long>(
            name: "PercentOff",
            schema: "Client",
            table: "DiscountRules",
            type: "bigint",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "numeric",
            oldNullable: true);

        migrationBuilder.AlterColumn<long>(
            name: "AmountOff",
            schema: "Client",
            table: "DiscountRules",
            type: "bigint",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "numeric",
            oldNullable: true);

        migrationBuilder.AddColumn<string>(
            name: "StripeId",
            schema: "Client",
            table: "DiscountRules",
            type: "text",
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "StripeId",
            schema: "Client",
            table: "DiscountRules");

        migrationBuilder.AlterColumn<decimal>(
            name: "PercentOff",
            schema: "Client",
            table: "DiscountRules",
            type: "numeric",
            nullable: true,
            oldClrType: typeof(long),
            oldType: "bigint",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "AmountOff",
            schema: "Client",
            table: "DiscountRules",
            type: "numeric",
            nullable: true,
            oldClrType: typeof(long),
            oldType: "bigint",
            oldNullable: true);
    }
}
