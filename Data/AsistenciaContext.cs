using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SistemaAsistencia.Models;


namespace SistemaAsistencia.Data
{
    public class AsistenciaContext : IdentityDbContext<ApplicationUser>
    {
        public AsistenciaContext(DbContextOptions<AsistenciaContext> options) : base(options) { }

        public DbSet<Estudiante> Estudiantes { get; set; }
        public DbSet<Profesor> Profesores { get; set; }
        public DbSet<Materia> Materias { get; set; }
        public DbSet<Seccion> Secciones { get; set; }
        public DbSet<Aula> Aulas { get; set; }
        public DbSet<Asistencia> Asistencias { get; set; }
        public DbSet<Justificacion> Justificaciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Unicidad de Estudiante
            modelBuilder.Entity<Estudiante>()
                .HasIndex(e => e.Codigo)
                .IsUnique();

            modelBuilder.Entity<Estudiante>()
                .HasIndex(e => e.Email)
                .IsUnique();

            modelBuilder.Entity<Estudiante>()
                .HasIndex(e => e.CuentaUsuarioId)
                .IsUnique()
                .HasFilter("[CuentaUsuarioId] IS NOT NULL");

            modelBuilder.Entity<Estudiante>()
                .HasOne<ApplicationUser>()
                .WithOne()
                .HasForeignKey<Estudiante>(e => e.CuentaUsuarioId)
                .OnDelete(DeleteBehavior.SetNull);

            // 2. Unicidad de Profesor
            modelBuilder.Entity<Profesor>()
                .HasIndex(p => p.Email)
                .IsUnique();

            modelBuilder.Entity<Profesor>()
                .HasIndex(p => p.CuentaUsuarioId)
                .IsUnique()
                .HasFilter("[CuentaUsuarioId] IS NOT NULL");

            modelBuilder.Entity<Profesor>()
                .HasOne<ApplicationUser>()
                .WithOne()
                .HasForeignKey<Profesor>(p => p.CuentaUsuarioId)
                .OnDelete(DeleteBehavior.SetNull);

            // 3. Unicidad de Materia; conservar el tamaño histórico de la columna con índice único.
            modelBuilder.Entity<Materia>()
                .Property(m => m.Codigo)
                .HasMaxLength(450);

            modelBuilder.Entity<Materia>()
                .HasIndex(m => m.Codigo)
                .IsUnique();

            modelBuilder.Entity<Aula>()
                .HasMany(a => a.Secciones)
                .WithOne(s => s.Aula)
                .HasForeignKey(s => s.AulaId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // 4. Prevenir doble asistencia del mismo alumno el mismo día en la misma sección
            modelBuilder.Entity<Asistencia>()
                .HasIndex(a => new { a.EstudianteId, a.SeccionId, a.Fecha })
                .IsUnique();
        }
    }
}