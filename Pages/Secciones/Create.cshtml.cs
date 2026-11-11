using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaAsistencia.Data;
using SistemaAsistencia.Models;

namespace SistemaAsistencia.Pages.Secciones;

public class CreateModel : PageModel
{
    private readonly AsistenciaContext _context;

    public CreateModel(AsistenciaContext context) => _context = context;

    [BindProperty]
    public Seccion Seccion { get; set; } = new();

    [BindProperty, Required(ErrorMessage = "Debe seleccionar un aula.")]
    public int? AulaId { get; set; }

    [BindProperty, Required(ErrorMessage = "Debe seleccionar una materia.")]
    public int? MateriaId { get; set; }

    [BindProperty, Required(ErrorMessage = "Debe seleccionar un profesor.")]
    public int? ProfesorId { get; set; }

    [BindProperty, Required(ErrorMessage = "Debe seleccionar un día de la semana.")]
    public DayOfWeek? Dia { get; set; }

    [BindProperty, Required(ErrorMessage = "La hora de inicio es obligatoria."), DataType(DataType.Time)]
    public TimeSpan? HoraInicio { get; set; }

    [BindProperty, Required(ErrorMessage = "La hora de fin es obligatoria."), DataType(DataType.Time)]
    public TimeSpan? HoraFin { get; set; }

    public SelectList? AulasList { get; private set; }
    public SelectList? MateriasList { get; private set; }
    public SelectList? ProfesoresList { get; private set; }
    public int AulasDisponibles { get; private set; }

    public IActionResult OnGet()
    {
        CargarListas();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (Dia.HasValue && !Enum.IsDefined(Dia.Value))
        {
            ModelState.AddModelError(nameof(Dia), "Selecciona un día válido.");
        }

        if (HoraInicio.HasValue && HoraFin.HasValue && HoraFin.Value <= HoraInicio.Value)
        {
            ModelState.AddModelError(nameof(HoraFin), "La hora de fin debe ser posterior a la hora de inicio.");
        }

        if (!ModelState.IsValid)
        {
            CargarListas();
            return Page();
        }

        if (!await _context.Aulas.AnyAsync(aula => aula.Id == AulaId!.Value))
            ModelState.AddModelError(nameof(AulaId), "El aula seleccionada ya no está disponible.");
        if (!await _context.Materias.AnyAsync(materia => materia.Id == MateriaId!.Value))
            ModelState.AddModelError(nameof(MateriaId), "La materia seleccionada ya no está disponible.");
        if (!await _context.Profesores.AnyAsync(profesor => profesor.Id == ProfesorId!.Value))
            ModelState.AddModelError(nameof(ProfesorId), "El profesor seleccionado ya no está disponible.");

        if (!ModelState.IsValid)
        {
            CargarListas();
            return Page();
        }

        Seccion.AulaId = AulaId!.Value;
        Seccion.MateriaId = MateriaId!.Value;
        Seccion.ProfesorId = ProfesorId!.Value;
        Seccion.Dia = Dia!.Value;
        Seccion.HoraInicio = HoraInicio!.Value;
        Seccion.HoraFin = HoraFin!.Value;

        var choqueHorario = await _context.Secciones.AnyAsync(seccion =>
            seccion.ProfesorId == ProfesorId.Value &&
            seccion.Dia == Dia.Value &&
            ((HoraInicio.Value >= seccion.HoraInicio && HoraInicio.Value < seccion.HoraFin) ||
             (HoraFin.Value > seccion.HoraInicio && HoraFin.Value <= seccion.HoraFin) ||
             (HoraInicio.Value <= seccion.HoraInicio && HoraFin.Value >= seccion.HoraFin)));

        if (choqueHorario)
        {
            ModelState.AddModelError(string.Empty, "El profesor ya tiene una clase asignada en ese día y horario.");
            CargarListas();
            return Page();
        }

        _context.Secciones.Add(Seccion);
        await _context.SaveChangesAsync();
        return RedirectToPage("./Index");
    }

    private void CargarListas()
    {
        var aulas = _context.Aulas.OrderBy(aula => aula.Nombre).ToList();
        AulasDisponibles = aulas.Count;
        AulasList = new SelectList(aulas, "Id", "Nombre", AulaId);
        MateriasList = new SelectList(_context.Materias.OrderBy(materia => materia.Nombre).ToList(), "Id", "Nombre", MateriaId);
        ProfesoresList = new SelectList(_context.Profesores.OrderBy(profesor => profesor.Nombre).ToList(), "Id", "Nombre", ProfesorId);
    }
}
