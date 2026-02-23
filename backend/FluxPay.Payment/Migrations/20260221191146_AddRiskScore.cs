using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FluxPay.Payment.Migrations
{
    /// <inheritdoc />
    public partial class AddRiskScore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RiskScore",
                table: "Payments",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RiskScore",
                table: "Payments");
        }
    }
}
