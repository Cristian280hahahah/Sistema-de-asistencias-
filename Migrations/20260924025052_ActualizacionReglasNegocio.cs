using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaAsistencia.Migrations
{
    /// <inheritdoc />
    public partial class ActualizacionReglasNegocio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Asistencias_EstudianteId",
                table: "Asistencias");

            migrationBuilder.RenameColumn(
                name: "Horario",
                table: "Secciones",
                newName: "Nombre");

            migrationBuilder.AddColumn<int>(
                name: "Dia",
                table: "Secciones",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "HoraFin",
                table: "Secciones",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "HoraInicio",
                table: "Secciones",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Profesores",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Profesores",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Codigo",
                table: "Materias",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Estudiantes",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Estudiantes",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Codigo",
                table: "Estudiantes",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "HoraLlegada",
                table: "Asistencias",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.CreateTable(
                name: "Justificaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Motivo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    AsistenciaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Justificaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Justificaciones_Asistencias_AsistenciaId",
                        column: x => x.AsistenciaId,
                        principalTable: "Asistencias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Profesores_Email",
                table: "Profesores",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Materias_Codigo",
                table: "Materias",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Estudiantes_Codigo",
                table: "Estudiantes",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Estudiantes_Email",
                table: "Estudiantes",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Asistencias_EstudianteId_SeccionId_Fecha",
                table: "Asistencias",
                columns: new[] { "EstudianteId", "SeccionId", "Fecha" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Justificaciones_AsistenciaId",
                table: "Justificaciones",
                column: "AsistenciaId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Justificaciones");

            migrationBuilder.DropIndex(
                name: "IX_Profesores_Email",
                table: "Profesores");

            migrationBuilder.DropIndex(
                name: "IX_Materias_Codigo",
                table: "Materias");

            migrationBuilder.DropIndex(
                name: "IX_Estudiantes_Codigo",
                table: "Estudiantes");

            migrationBuilder.DropIndex(
                name: "IX_Estudiantes_Email",
                table: "Estudiantes");

            migrationBuilder.DropIndex(
                name: "IX_Asistencias_EstudianteId_SeccionId_Fecha",
                table: "Asistencias");

            migrationBuilder.DropColumn(
                name: "Dia",
                table: "Secciones");

            migrationBuilder.DropColumn(
                name: "HoraFin",
                table: "Secciones");

            migrationBuilder.DropColumn(
                name: "HoraInicio",
                table: "Secciones");

            migrationBuilder.DropColumn(
                name: "HoraLlegada",
                table: "Asistencias");

            migrationBuilder.RenameColumn(
                name: "Nombre",
                table: "Secciones",
                newName: "Horario");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Profesores",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Profesores",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Codigo",
                table: "Materias",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Estudiantes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Estudiantes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Codigo",
                table: "Estudiantes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_Asistencias_EstudianteId",
                table: "Asistencias",
                column: "EstudianteId");
        }
    }
}
