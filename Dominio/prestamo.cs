
namespace PrestamosBiblioteca.Dominio;

public class Prestamo
{
    public int Id { get; set; }
    public int LectorId { get; set; }
    public int LibroId { get; set; }
    public DateTime FechaPrestamo { get; set; }
    public DateTime FechaLimite { get; set; }
    public DateTime? FechaDevolucion { get; set; }

    public bool EstaActivo
    {
        get { return FechaDevolucion == null; }
    }
}
