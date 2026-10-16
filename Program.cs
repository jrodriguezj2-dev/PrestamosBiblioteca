
using Microsoft.Data.Sqlite;

class Program
{
    static string cadenaConexion = "Data Source=Biblioteca.db";

    static void Main()
    {
        CrearBaseDatos();

        int opcion = 0;

        while (opcion != 7)
        {
            Console.WriteLine("\n===== BIBLIOTECA =====");
            Console.WriteLine("1. Registrar libro");
            Console.WriteLine("2. Mostrar libros");
            Console.WriteLine("3. Registrar usuario");
            Console.WriteLine("4. Registrar prestamo");
            Console.WriteLine("5. Registrar devolucion");
            Console.WriteLine("6. Consultar prestamos");
            Console.WriteLine("7. Salir");
            Console.Write("Seleccione una opcion: ");

            if (!int.TryParse(Console.ReadLine(), out opcion))
            {
                Console.WriteLine("Ingrese un numero valido.");
                opcion = 0;
                continue;
            }

            switch (opcion)
            {
                case 1:
                    RegistrarLibro();
                    break;
                case 2:
                    MostrarLibros();
                    break;
                case 3:
                    RegistrarUsuario();
                    break;
                case 4:
                    RegistrarPrestamo();
                    break;
                case 5:
                    RegistrarDevolucion();
                    break;
                case 6:
                    ConsultarPrestamos();
                    break;
                case 7:
                    Console.WriteLine("Hasta pronto.");
                    break;
                default:
                    Console.WriteLine("Opcion no valida.");
                    break;
            }
        }
    }

    static void CrearBaseDatos()
    {
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        string sql = @"
        CREATE TABLE IF NOT EXISTS Libros (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Titulo TEXT NOT NULL,
            Autor TEXT NOT NULL,
            Cantidad INTEGER NOT NULL,
            Disponibles INTEGER NOT NULL
        );

        CREATE TABLE IF NOT EXISTS Usuarios (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Nombre TEXT NOT NULL
        );

        CREATE TABLE IF NOT EXISTS Prestamos (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            LibroId INTEGER NOT NULL,
            UsuarioId INTEGER NOT NULL,
            FechaPrestamo TEXT NOT NULL,
            Estado TEXT NOT NULL,
            FOREIGN KEY (LibroId) REFERENCES Libros(Id),
            FOREIGN KEY (UsuarioId) REFERENCES Usuarios(Id)
        );";

        using var comando = new SqliteCommand(sql, conexion);
        comando.ExecuteNonQuery();
    }

    static void RegistrarLibro()
    {
        Console.Write("Titulo del libro: ");
        string titulo = Console.ReadLine() ?? "";

        Console.Write("Autor: ");
        string autor = Console.ReadLine() ?? "";

        Console.Write("Cantidad de ejemplares: ");
        if (!int.TryParse(Console.ReadLine(), out int cantidad) ||
            cantidad <= 0)
        {
            Console.WriteLine("La cantidad debe ser mayor que cero.");
            return;
        }

        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        string sql = @"INSERT INTO Libros
            (Titulo, Autor, Cantidad, Disponibles)
            VALUES (@titulo, @autor, @cantidad, @cantidad)";

        using var comando = new SqliteCommand(sql, conexion);
        comando.Parameters.AddWithValue("@titulo", titulo);
        comando.Parameters.AddWithValue("@autor", autor);
        comando.Parameters.AddWithValue("@cantidad", cantidad);
        comando.ExecuteNonQuery();

        Console.WriteLine("Libro registrado correctamente.");
    }

    static void MostrarLibros()
    {
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        string sql = "SELECT * FROM Libros";

        using var comando = new SqliteCommand(sql, conexion);
        using var lector = comando.ExecuteReader();

        Console.WriteLine("\n--- LIBROS REGISTRADOS ---");

        while (lector.Read())
        {
            Console.WriteLine(
                $"ID: {lector["Id"]} | " +
                $"Titulo: {lector["Titulo"]} | " +
                $"Autor: {lector["Autor"]} | " +
                $"Disponibles: {lector["Disponibles"]}");
        }
    }

    static void RegistrarUsuario()
    {
        Console.Write("Nombre del usuario: ");
        string nombre = Console.ReadLine() ?? "";

        if (string.IsNullOrWhiteSpace(nombre))
        {
            Console.WriteLine("El nombre es obligatorio.");
            return;
        }

        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        string sql = "INSERT INTO Usuarios (Nombre) VALUES (@nombre)";

        using var comando = new SqliteCommand(sql, conexion);
        comando.Parameters.AddWithValue("@nombre", nombre);
        comando.ExecuteNonQuery();

        Console.WriteLine("Usuario registrado correctamente.");
    }

    static void RegistrarPrestamo()
    {
        Console.Write("ID del libro: ");
        if (!int.TryParse(Console.ReadLine(), out int libroId))
        {
            Console.WriteLine("ID no valido.");
            return;
        }

        Console.Write("ID del usuario: ");
        if (!int.TryParse(Console.ReadLine(), out int usuarioId))
        {
            Console.WriteLine("ID no valido.");
            return;
        }

        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        using var transaccion = conexion.BeginTransaction();

        string buscarLibro =
            "SELECT Disponibles FROM Libros WHERE Id = @id";

        using var comandoLibro =
            new SqliteCommand(buscarLibro, conexion, transaccion);
        comandoLibro.Parameters.AddWithValue("@id", libroId);

        object? resultadoLibro = comandoLibro.ExecuteScalar();

        if (resultadoLibro == null ||
            Convert.ToInt32(resultadoLibro) <= 0)
        {
            Console.WriteLine("El libro no existe o no esta disponible.");
            transaccion.Rollback();
            return;
        }

        string buscarUsuario =
            "SELECT COUNT(*) FROM Usuarios WHERE Id = @id";

        using var comandoUsuario =
            new SqliteCommand(buscarUsuario, conexion, transaccion);
        comandoUsuario.Parameters.AddWithValue("@id", usuarioId);

        if (Convert.ToInt32(comandoUsuario.ExecuteScalar()) == 0)
        {
            Console.WriteLine("El usuario no existe.");
            transaccion.Rollback();
            return;
        }

        string insertar = @"
            INSERT INTO Prestamos
            (LibroId, UsuarioId, FechaPrestamo, Estado)
            VALUES (@libro, @usuario, @fecha, 'Prestado')";

        using var comandoPrestamo =
            new SqliteCommand(insertar, conexion, transaccion);
        comandoPrestamo.Parameters.AddWithValue("@libro", libroId);
        comandoPrestamo.Parameters.AddWithValue("@usuario", usuarioId);
        comandoPrestamo.Parameters.AddWithValue(
            "@fecha", DateTime.Now.ToString("yyyy-MM-dd"));

        comandoPrestamo.ExecuteNonQuery();

        string actualizar =
            "UPDATE Libros SET Disponibles = Disponibles - 1 WHERE Id = @id";

        using var comandoActualizar =
            new SqliteCommand(actualizar, conexion, transaccion);
        comandoActualizar.Parameters.AddWithValue("@id", libroId);
        comandoActualizar.ExecuteNonQuery();

        transaccion.Commit();

        Console.WriteLine("Prestamo registrado correctamente.");
    }

    static void RegistrarDevolucion()
    {
        Console.Write("ID del prestamo: ");
        if (!int.TryParse(Console.ReadLine(), out int prestamoId))
        {
            Console.WriteLine("ID no valido.");
            return;
        }

        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        using var transaccion = conexion.BeginTransaction();

        string buscar =
            "SELECT LibroId FROM Prestamos " +
            "WHERE Id = @id AND Estado = 'Prestado'";

        using var comandoBuscar =
            new SqliteCommand(buscar, conexion, transaccion);
        comandoBuscar.Parameters.AddWithValue("@id", prestamoId);

        object? resultado = comandoBuscar.ExecuteScalar();

        if (resultado == null)
        {
            Console.WriteLine("Prestamo inexistente o ya devuelto.");
            transaccion.Rollback();
            return;
        }

        int libroId = Convert.ToInt32(resultado);

        string devolver =
            "UPDATE Prestamos SET Estado = 'Devuelto' WHERE Id = @id";

        using var comandoDevolver =
            new SqliteCommand(devolver, conexion, transaccion);
        comandoDevolver.Parameters.AddWithValue("@id", prestamoId);
        comandoDevolver.ExecuteNonQuery();

        string actualizarLibro =
            "UPDATE Libros SET Disponibles = Disponibles + 1 WHERE Id = @id";

        using var comandoLibro =
            new SqliteCommand(actualizarLibro, conexion, transaccion);
        comandoLibro.Parameters.AddWithValue("@id", libroId);
        comandoLibro.ExecuteNonQuery();

        transaccion.Commit();

        Console.WriteLine("Devolucion registrada correctamente.");
    }

    static void ConsultarPrestamos()
    {
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        string sql = @"
        SELECT P.Id, L.Titulo, U.Nombre,
               P.FechaPrestamo, P.Estado
        FROM Prestamos P
        INNER JOIN Libros L ON P.LibroId = L.Id
        INNER JOIN Usuarios U ON P.UsuarioId = U.Id
        ORDER BY P.Id";

        using var comando = new SqliteCommand(sql, conexion);
        using var lector = comando.ExecuteReader();

        Console.WriteLine("\n--- HISTORIAL DE PRESTAMOS ---");

        while (lector.Read())
        {
            Console.WriteLine(
                $"Prestamo: {lector["Id"]} | " +
                $"Libro: {lector["Titulo"]} | " +
                $"Usuario: {lector["Nombre"]} | " +
                $"Fecha: {lector["FechaPrestamo"]} | " +
                $"Estado: {lector["Estado"]}");
        }
    }
}
