
using PrestamosBiblioteca.Dominio;
using PrestamosBiblioteca.Persistencia;

namespace PrestamosBiblioteca.Aplicacion;

public class MenuBiblioteca
{
    private readonly RepositorioBiblioteca repositorio;

    public MenuBiblioteca(RepositorioBiblioteca repositorio)
    {
        this.repositorio = repositorio;
    }

    public void Ejecutar()
    {
        int opcion;

        do
        {
            Console.WriteLine("\n===== SISTEMA DE BIBLIOTECA =====");
            Console.WriteLine("1. Registrar lector");
            Console.WriteLine("2. Mostrar lectores");
            Console.WriteLine("3. Actualizar lector");
            Console.WriteLine("4. Eliminar lector");
            Console.WriteLine("5. Registrar libro");
            Console.WriteLine("6. Mostrar libros");
            Console.WriteLine("7. Actualizar libro");
            Console.WriteLine("8. Eliminar libro");
            Console.WriteLine("9. Registrar préstamo");
            Console.WriteLine("10. Consultar préstamos");
            Console.WriteLine("11. Registrar devolución");
            Console.WriteLine("12. Demostrar polimorfismo");
            Console.WriteLine("0. Salir");
            Console.Write("Seleccione una opción: ");

            if (!int.TryParse(Console.ReadLine(), out opcion))
            {
                Console.WriteLine("Ingrese un número válido.");
                opcion = -1;
                continue;
            }

            try
            {
                switch (opcion)
                {
                    case 1:
                        CrearLector();
                        break;
                    case 2:
                        MostrarLectores();
                        break;
                    case 3:
                        ActualizarLector();
                        break;
                    case 4:
                        EliminarLector();
                        break;
                    case 5:
                        CrearLibro();
                        break;
                    case 6:
                        MostrarLibros();
                        break;
                    case 7:
                        ActualizarLibro();
                        break;
                    case 8:
                        EliminarLibro();
                        break;
                    case 9:
                        CrearPrestamo();
                        break;
                    case 10:
                        MostrarPrestamos();
                        break;
                    case 11:
                        RegistrarDevolucion();
                        break;
                    case 12:
                        DemostrarPolimorfismo();
                        break;
                    case 0:
                        Console.WriteLine("Hasta pronto.");
                        break;
                    default:
                        Console.WriteLine("Opción no válida.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

        } while (opcion != 0);
    }

    private void CrearLector()
    {
        var lector = new Lector
        {
            Codigo = LeerTexto("Código del lector: "),
            Nombre = LeerTexto("Nombre: "),
            Correo = LeerTexto("Correo: "),
            Telefono = LeerTexto("Teléfono: ")
        };

        repositorio.CrearLector(lector);
        Console.WriteLine("Lector registrado correctamente.");
    }

    private void MostrarLectores()
    {
        var lectores = repositorio.ObtenerLectores();

        Console.WriteLine("\n--- LECTORES REGISTRADOS ---");

        if (lectores.Count == 0)
        {
            Console.WriteLine("No hay lectores registrados.");
            return;
        }

        foreach (var lector in lectores)
        {
            Console.WriteLine(
                $"{lector.Id}. {lector.Codigo} | " +
                $"{lector.Nombre} | {lector.Correo} | " +
                $"{lector.Telefono}");
        }
    }

    private void ActualizarLector()
    {
        MostrarLectores();

        var lector = new Lector
        {
            Id = LeerEntero("ID del lector: "),
            Codigo = LeerTexto("Nuevo código: "),
            Nombre = LeerTexto("Nuevo nombre: "),
            Correo = LeerTexto("Nuevo correo: "),
            Telefono = LeerTexto("Nuevo teléfono: ")
        };

        repositorio.ActualizarLector(lector);
        Console.WriteLine("Lector actualizado.");
    }

    private void EliminarLector()
    {
        MostrarLectores();

        int id = LeerEntero("ID del lector que desea eliminar: ");

        repositorio.EliminarLector(id);
        Console.WriteLine("Lector eliminado.");
    }

    private void CrearLibro()
    {
        var libro = new Libro
        {
            Codigo = LeerTexto("Código del libro: "),
            Titulo = LeerTexto("Título: "),
            Autor = LeerTexto("Autor: "),
            Ejemplares = LeerEntero("Cantidad de ejemplares: ")
        };

        if (libro.Ejemplares <= 0)
            throw new Exception("Debe registrar al menos un ejemplar.");

        repositorio.CrearLibro(libro);
        Console.WriteLine("Libro registrado correctamente.");
    }

    private void MostrarLibros()
    {
        var libros = repositorio.ObtenerLibros();

        Console.WriteLine("\n--- CATÁLOGO DE LIBROS ---");

        if (libros.Count == 0)
        {
            Console.WriteLine("No hay libros registrados.");
            return;
        }

        foreach (var libro in libros)
        {
            Console.WriteLine(
                $"{libro.Id}. {libro.Codigo} | " +
                $"{libro.Titulo} | {libro.Autor} | " +
                $"Disponibles: {libro.Ejemplares}");
        }
    }

    private void ActualizarLibro()
    {
        MostrarLibros();

        var libro = new Libro
        {
            Id = LeerEntero("ID del libro: "),
            Codigo = LeerTexto("Nuevo código: "),
            Titulo = LeerTexto("Nuevo título: "),
            Autor = LeerTexto("Nuevo autor: "),
            Ejemplares = LeerEntero("Cantidad de ejemplares: ")
        };

        if (libro.Ejemplares < 0)
            throw new Exception("La cantidad no puede ser negativa.");

        repositorio.ActualizarLibro(libro);
        Console.WriteLine("Libro actualizado.");
    }

    private void EliminarLibro()
    {
        MostrarLibros();

        int id = LeerEntero("ID del libro que desea eliminar: ");

        repositorio.EliminarLibro(id);
        Console.WriteLine("Libro eliminado.");
    }

    private void CrearPrestamo()
    {
        Console.WriteLine("\n--- LECTORES ---");
        MostrarLectores();

        Console.WriteLine("\n--- LIBROS ---");
        MostrarLibros();

        var prestamo = new Prestamo
        {
            LectorId = LeerEntero("ID del lector: "),
            LibroId = LeerEntero("ID del libro: "),
            FechaPrestamo = DateTime.Today,
            FechaLimite = LeerFecha("Fecha límite (AAAA-MM-DD): ")
        };

        if (prestamo.FechaLimite < prestamo.FechaPrestamo)
            throw new Exception(
                "La fecha límite no puede ser anterior a hoy.");

        repositorio.CrearPrestamo(prestamo);
        Console.WriteLine("Préstamo registrado correctamente.");
    }

    private void MostrarPrestamos()
    {
        var prestamos = repositorio.ObtenerPrestamos();

        Console.WriteLine("\n--- HISTORIAL DE PRÉSTAMOS ---");

        if (prestamos.Count == 0)
        {
            Console.WriteLine("No hay préstamos registrados.");
            return;
        }

        foreach (string prestamo in prestamos)
        {
            Console.WriteLine(prestamo);
        }
    }

    private void RegistrarDevolucion()
    {
        MostrarPrestamos();

        int id = LeerEntero(
            "ID del préstamo que desea devolver: ");

        repositorio.RegistrarDevolucion(id);
        Console.WriteLine("Devolución registrada correctamente.");
    }

    private void DemostrarPolimorfismo()
    {
        Persona persona = new Lector
        {
            Id = 1,
            Codigo = "L001",
            Nombre = "Ana López",
            Correo = "ana@correo.com",
            Telefono = "55555555"
        };

        Console.WriteLine("\n--- DEMOSTRACIÓN DE POLIMORFISMO ---");
        Console.WriteLine(persona.ObtenerInformacion());
    }

    private static string LeerTexto(string mensaje)
    {
        Console.Write(mensaje);
        string? valor = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(valor))
            throw new Exception("El campo es obligatorio.");

        return valor.Trim();
    }

    private static int LeerEntero(string mensaje)
    {
        Console.Write(mensaje);

        if (!int.TryParse(Console.ReadLine(), out int valor))
            throw new Exception(
                "Debe ingresar un número entero válido.");

        return valor;
    }

    private static DateTime LeerFecha(string mensaje)
    {
        Console.Write(mensaje);

        if (!DateTime.TryParse(
            Console.ReadLine(), out DateTime fecha))
        {
            throw new Exception(
                "Ingrese una fecha válida, por ejemplo 2026-10-20.");
        }

        return fecha.Date;
    }
}
