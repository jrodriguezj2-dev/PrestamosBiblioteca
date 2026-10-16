
namespace PrestamosBiblioteca.Dominio;

public class Lector : Persona
{
    public string Codigo { get; set; } = "";
    public string Correo { get; set; } = "";
    public string Telefono { get; set; } = "";

    public override string ObtenerInformacion()
    {
        return $"Código: {Codigo} - Nombre: {Nombre} - " +
               $"Correo: {Correo} - Teléfono: {Telefono}";
    }
}
