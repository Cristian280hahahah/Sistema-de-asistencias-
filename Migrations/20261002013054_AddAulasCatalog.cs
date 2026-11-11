using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaAsistencia.Migrations
{
    /// <inheritdoc />
    public partial class AddAulasCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AulaId",
                table: "Secciones",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Aulas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ubicacion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aulas", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Secciones_AulaId",
                table: "Secciones",
                column: "AulaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Secciones_Aulas_AulaId",
                table: "Secciones",
                column: "AulaId",
                principalTable: "Aulas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.Sql("""
                INSERT INTO [Aulas] ([Nombre])
                SELECT seccion.[Nombre]
                FROM [Secciones] AS seccion
                WHERE seccion.[Nombre] IS NOT NULL
                  AND LEN(LTRIM(RTRIM(seccion.[Nombre]))) > 0
                  AND NOT EXISTS (
                      SELECT 1
                      FROM [Secciones] AS anterior
                      WHERE anterior.[Id] < seccion.[Id]
                        AND anterior.[Nombre] = seccion.[Nombre]);

                UPDATE seccion
                SET [AulaId] = aula.[Id]
                FROM [Secciones] AS seccion
                INNER JOIN [Aulas] AS aula ON aula.[Nombre] = seccion.[Nombre];
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Secciones_Aulas_AulaId",
                table: "Secciones");

            migrationBuilder.DropTable(
                name: "Aulas");

            migrationBuilder.DropIndex(
                name: "IX_Secciones_AulaId",
                table: "Secciones");

            migrationBuilder.DropColumn(
                name: "AulaId",
                table: "Secciones");
        }
    }
}
