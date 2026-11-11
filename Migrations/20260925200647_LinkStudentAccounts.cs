using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaAsistencia.Migrations
{
    /// <inheritdoc />
    public partial class LinkStudentAccounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CuentaUsuarioId",
                table: "Estudiantes",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Estudiantes_CuentaUsuarioId",
                table: "Estudiantes",
                column: "CuentaUsuarioId",
                unique: true,
                filter: "[CuentaUsuarioId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Estudiantes_AspNetUsers_CuentaUsuarioId",
                table: "Estudiantes",
                column: "CuentaUsuarioId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Estudiantes_AspNetUsers_CuentaUsuarioId",
                table: "Estudiantes");

            migrationBuilder.DropIndex(
                name: "IX_Estudiantes_CuentaUsuarioId",
                table: "Estudiantes");

            migrationBuilder.DropColumn(
                name: "CuentaUsuarioId",
                table: "Estudiantes");
        }
    }
}
