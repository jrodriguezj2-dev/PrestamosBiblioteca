
using PrestamosBiblioteca.Persistencia;
using PrestamosBiblioteca.Aplicacion;

class Program
{
    static void Main()
    {
        string cadenaConexion = "Data Source=Biblioteca.db";

        var repositorio = new RepositorioBiblioteca(cadenaConexion);

        var menu = new MenuBiblioteca(repositorio);

        menu.Ejecutar();
    }
}
