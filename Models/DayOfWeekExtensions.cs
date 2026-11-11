namespace SistemaAsistencia.Models;

public static class DayOfWeekExtensions
{
    public static string NombreEnEspanol(this DayOfWeek day) => day switch
    {
        DayOfWeek.Sunday => "Domingo",
        DayOfWeek.Monday => "Lunes",
        DayOfWeek.Tuesday => "Martes",
        DayOfWeek.Wednesday => "Miércoles",
        DayOfWeek.Thursday => "Jueves",
        DayOfWeek.Friday => "Viernes",
        DayOfWeek.Saturday => "Sábado",
        _ => throw new ArgumentOutOfRangeException(nameof(day), day, null)
    };
}
