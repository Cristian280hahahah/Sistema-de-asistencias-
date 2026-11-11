using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaAsistencia.Migrations
{
    /// <inheritdoc />
    public partial class RequireAulaForSections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE seccion
                SET [AulaId] = aula.[Id]
                FROM [Secciones] AS seccion
                INNER JOIN [Aulas] AS aula ON aula.[Nombre] = seccion.[Nombre]
                WHERE seccion.[AulaId] IS NULL
                  AND LEN(LTRIM(RTRIM(seccion.[Nombre]))) > 0;

                IF EXISTS (SELECT 1 FROM [Secciones] WHERE [AulaId] IS NULL)
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM [Aulas] WHERE [Nombre] = N'Aula sin asignar')
                        INSERT INTO [Aulas] ([Nombre]) VALUES (N'Aula sin asignar');

                    DECLARE @AulaSinAsignarId int = (
                        SELECT TOP (1) [Id]
                        FROM [Aulas]
                        WHERE [Nombre] = N'Aula sin asignar'
                        ORDER BY [Id]);

                    UPDATE [Secciones]
                    SET [AulaId] = @AulaSinAsignarId
                    WHERE [AulaId] IS NULL;
                END;
                """);

            migrationBuilder.DropColumn(
                name: "Nombre",
                table: "Secciones");

            migrationBuilder.AlterColumn<int>(
                name: "AulaId",
                table: "Secciones",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "AulaId",
                table: "Secciones",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Nombre",
                table: "Secciones",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("""
                UPDATE seccion
                SET [Nombre] = aula.[Nombre]
                FROM [Secciones] AS seccion
                INNER JOIN [Aulas] AS aula ON aula.[Id] = seccion.[AulaId];
                """);
        }
    }
}
