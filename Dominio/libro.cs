
namespace PrestamosBiblioteca.Dominio;

public class Libro
{
    public int Id { get; set; }
    public string Codigo { get; set; } = "";
    public string Titulo { get; set; } = "";
    public string Autor { get; set; } = "";
    public int Ejemplares { get; set; }
}
