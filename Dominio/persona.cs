
namespace PrestamosBiblioteca.Dominio;

public class Persona
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";

    public virtual string ObtenerInformacion()
    {
        return $"ID: {Id} - Nombre: {Nombre}";
    }
}
