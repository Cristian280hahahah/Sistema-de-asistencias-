using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaAsistencia.Migrations
{
    /// <inheritdoc />
    public partial class AddProfessorUserLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CuentaUsuarioId",
                table: "Profesores",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Profesores_CuentaUsuarioId",
                table: "Profesores",
                column: "CuentaUsuarioId",
                unique: true,
                filter: "[CuentaUsuarioId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Profesores_AspNetUsers_CuentaUsuarioId",
                table: "Profesores",
                column: "CuentaUsuarioId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Profesores_AspNetUsers_CuentaUsuarioId",
                table: "Profesores");

            migrationBuilder.DropIndex(
                name: "IX_Profesores_CuentaUsuarioId",
                table: "Profesores");

            migrationBuilder.DropColumn(
                name: "CuentaUsuarioId",
                table: "Profesores");
        }
    }
}
