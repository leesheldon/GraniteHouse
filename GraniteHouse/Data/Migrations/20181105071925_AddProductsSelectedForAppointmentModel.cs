using Microsoft.EntityFrameworkCore.Migrations;

namespace GraniteHouse.Data.Migrations
{
    public partial class AddProductsSelectedForAppointmentModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductsSelectedForAppointment",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    AppointmentId = table.Column<string>(nullable: true),
                    ProductId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductsSelectedForAppointment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductsSelectedForAppointment_Appointments_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductsSelectedForAppointment_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductsSelectedForAppointment_AppointmentId",
                table: "ProductsSelectedForAppointment",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductsSelectedForAppointment_ProductId",
                table: "ProductsSelectedForAppointment",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductsSelectedForAppointment");
        }
    }
}
