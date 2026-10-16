
using Microsoft.Data.Sqlite;
using PrestamosBiblioteca.Dominio;

namespace PrestamosBiblioteca.Persistencia;

public class RepositorioBiblioteca
{
    private readonly string cadenaConexion;

    public RepositorioBiblioteca(string cadenaConexion)
    {
        this.cadenaConexion = cadenaConexion;
        CrearTablas();
    }

    private SqliteConnection Conectar()
    {
        var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        using var comando = conexion.CreateCommand();
        comando.CommandText = "PRAGMA foreign_keys = ON;";
        comando.ExecuteNonQuery();

        return conexion;
    }

    private void CrearTablas()
    {
        using var conexion = Conectar();
        using var comando = conexion.CreateCommand();

        comando.CommandText = @"
        CREATE TABLE IF NOT EXISTS Lectores (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Codigo TEXT NOT NULL UNIQUE,
            Nombre TEXT NOT NULL,
            Correo TEXT NOT NULL,
            Telefono TEXT NOT NULL
        );

        CREATE TABLE IF NOT EXISTS Libros (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Codigo TEXT NOT NULL UNIQUE,
            Titulo TEXT NOT NULL,
            Autor TEXT NOT NULL,
            Ejemplares INTEGER NOT NULL CHECK(Ejemplares >= 0)
        );

        CREATE TABLE IF NOT EXISTS Prestamos (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            LectorId INTEGER NOT NULL,
            LibroId INTEGER NOT NULL,
            FechaPrestamo TEXT NOT NULL,
            FechaLimite TEXT NOT NULL,
            FechaDevolucion TEXT NULL,
            FOREIGN KEY(LectorId) REFERENCES Lectores(Id),
            FOREIGN KEY(LibroId) REFERENCES Libros(Id)
        );";

        comando.ExecuteNonQuery();
    }

    // REGISTRAR LECTOR
    public void CrearLector(Lector lector)
    {
        using var conexion = Conectar();
        using var comando = conexion.CreateCommand();

        comando.CommandText = @"
        INSERT INTO Lectores
        (Codigo, Nombre, Correo, Telefono)
        VALUES ($codigo, $nombre, $correo, $telefono)";

        comando.Parameters.AddWithValue("$codigo", lector.Codigo);
        comando.Parameters.AddWithValue("$nombre", lector.Nombre);
        comando.Parameters.AddWithValue("$correo", lector.Correo);
        comando.Parameters.AddWithValue("$telefono", lector.Telefono);

        comando.ExecuteNonQuery();
    }

    // CONSULTAR LECTORES
    public List<Lector> ObtenerLectores()
    {
        var lista = new List<Lector>();

        using var conexion = Conectar();
        using var comando = conexion.CreateCommand();

        comando.CommandText = "SELECT * FROM Lectores ORDER BY Id";

        using var lector = comando.ExecuteReader();

        while (lector.Read())
        {
            lista.Add(new Lector
            {
                Id = lector.GetInt32(0),
                Codigo = lector.GetString(1),
                Nombre = lector.GetString(2),
                Correo = lector.GetString(3),
                Telefono = lector.GetString(4)
            });
        }

        return lista;
    }

    // ACTUALIZAR LECTOR
    public void ActualizarLector(Lector lector)
    {
        using var conexion = Conectar();
        using var comando = conexion.CreateCommand();

        comando.CommandText = @"
        UPDATE Lectores
        SET Codigo=$codigo, Nombre=$nombre,
            Correo=$correo, Telefono=$telefono
        WHERE Id=$id";

        comando.Parameters.AddWithValue("$id", lector.Id);
        comando.Parameters.AddWithValue("$codigo", lector.Codigo);
        comando.Parameters.AddWithValue("$nombre", lector.Nombre);
        comando.Parameters.AddWithValue("$correo", lector.Correo);
        comando.Parameters.AddWithValue("$telefono", lector.Telefono);

        if (comando.ExecuteNonQuery() == 0)
            throw new Exception("No se encontró el lector.");
    }

    // ELIMINAR LECTOR
    public void EliminarLector(int id)
    {
        using var conexion = Conectar();
        using var comando = conexion.CreateCommand();

        comando.CommandText = @"
        DELETE FROM Lectores
        WHERE Id=$id
        AND NOT EXISTS (
            SELECT 1 FROM Prestamos WHERE LectorId=$id
        )";

        comando.Parameters.AddWithValue("$id", id);

        if (comando.ExecuteNonQuery() == 0)
            throw new Exception(
                "No existe el lector o tiene préstamos registrados.");
    }

    // REGISTRAR LIBRO
    public void CrearLibro(Libro libro)
    {
        using var conexion = Conectar();
        using var comando = conexion.CreateCommand();

        comando.CommandText = @"
        INSERT INTO Libros (Codigo, Titulo, Autor, Ejemplares)
        VALUES ($codigo, $titulo, $autor, $ejemplares)";

        comando.Parameters.AddWithValue("$codigo", libro.Codigo);
        comando.Parameters.AddWithValue("$titulo", libro.Titulo);
        comando.Parameters.AddWithValue("$autor", libro.Autor);
        comando.Parameters.AddWithValue("$ejemplares", libro.Ejemplares);

        comando.ExecuteNonQuery();
    }

    // CONSULTAR LIBROS
    public List<Libro> ObtenerLibros()
    {
        var lista = new List<Libro>();

        using var conexion = Conectar();
        using var comando = conexion.CreateCommand();

        comando.CommandText = "SELECT * FROM Libros ORDER BY Id";

        using var lector = comando.ExecuteReader();

        while (lector.Read())
        {
            lista.Add(new Libro
            {
                Id = lector.GetInt32(0),
                Codigo = lector.GetString(1),
                Titulo = lector.GetString(2),
                Autor = lector.GetString(3),
                Ejemplares = lector.GetInt32(4)
            });
        }

        return lista;
    }

    // ACTUALIZAR LIBRO
    public void ActualizarLibro(Libro libro)
    {
        using var conexion = Conectar();
        using var comando = conexion.CreateCommand();

        comando.CommandText = @"
        UPDATE Libros
        SET Codigo=$codigo, Titulo=$titulo,
            Autor=$autor, Ejemplares=$ejemplares
        WHERE Id=$id";

        comando.Parameters.AddWithValue("$id", libro.Id);
        comando.Parameters.AddWithValue("$codigo", libro.Codigo);
        comando.Parameters.AddWithValue("$titulo", libro.Titulo);
        comando.Parameters.AddWithValue("$autor", libro.Autor);
        comando.Parameters.AddWithValue("$ejemplares", libro.Ejemplares);

        if (comando.ExecuteNonQuery() == 0)
            throw new Exception("No se encontró el libro.");
    }

    // ELIMINAR LIBRO
    public void EliminarLibro(int id)
    {
        using var conexion = Conectar();
        using var comando = conexion.CreateCommand();

        comando.CommandText = @"
        DELETE FROM Libros
        WHERE Id=$id
        AND NOT EXISTS (
            SELECT 1 FROM Prestamos WHERE LibroId=$id
        )";

        comando.Parameters.AddWithValue("$id", id);

        if (comando.ExecuteNonQuery() == 0)
            throw new Exception(
                "No existe el libro o tiene préstamos registrados.");
    }

    // REGISTRAR PRÉSTAMO
    public void CrearPrestamo(Prestamo prestamo)
    {
        using var conexion = Conectar();
        using var transaccion = conexion.BeginTransaction();

        using var comprobarLector = conexion.CreateCommand();
        comprobarLector.Transaction = transaccion;
        comprobarLector.CommandText =
            "SELECT COUNT(*) FROM Lectores WHERE Id=$id";
        comprobarLector.Parameters.AddWithValue("$id", prestamo.LectorId);

        if (Convert.ToInt32(comprobarLector.ExecuteScalar()) == 0)
            throw new Exception("El lector no existe.");

        using var actualizarLibro = conexion.CreateCommand();
        actualizarLibro.Transaction = transaccion;
        actualizarLibro.CommandText = @"
        UPDATE Libros
        SET Ejemplares = Ejemplares - 1
        WHERE Id=$id AND Ejemplares > 0";
        actualizarLibro.Parameters.AddWithValue("$id", prestamo.LibroId);

        if (actualizarLibro.ExecuteNonQuery() == 0)
            throw new Exception("El libro no existe o no está disponible.");

        using var comando = conexion.CreateCommand();
        comando.Transaction = transaccion;
        comando.CommandText = @"
        INSERT INTO Prestamos
        (LectorId, LibroId, FechaPrestamo, FechaLimite)
        VALUES ($lector, $libro, $fecha, $limite)";

        comando.Parameters.AddWithValue("$lector", prestamo.LectorId);
        comando.Parameters.AddWithValue("$libro", prestamo.LibroId);
        comando.Parameters.AddWithValue(
            "$fecha", prestamo.FechaPrestamo.ToString("yyyy-MM-dd"));
        comando.Parameters.AddWithValue(
            "$limite", prestamo.FechaLimite.ToString("yyyy-MM-dd"));

        comando.ExecuteNonQuery();
        transaccion.Commit();
    }

    // CONSULTAR PRÉSTAMOS
    public List<string> ObtenerPrestamos()
    {
        var lista = new List<string>();

        using var conexion = Conectar();
        using var comando = conexion.CreateCommand();

        comando.CommandText = @"
        SELECT P.Id, L.Nombre, B.Titulo,
               P.FechaPrestamo, P.FechaLimite, P.FechaDevolucion
        FROM Prestamos P
        INNER JOIN Lectores L ON P.LectorId=L.Id
        INNER JOIN Libros B ON P.LibroId=B.Id
        ORDER BY P.Id";

        using var lector = comando.ExecuteReader();

        while (lector.Read())
        {
            string estado = lector.IsDBNull(5)
                ? "Pendiente"
                : "Devuelto";

            lista.Add(
                $"Préstamo: {lector.GetInt32(0)} | " +
                $"Lector: {lector.GetString(1)} | " +
                $"Libro: {lector.GetString(2)} | " +
                $"Fecha: {lector.GetString(3)} | " +
                $"Límite: {lector.GetString(4)} | " +
                $"Estado: {estado}");
        }

        return lista;
    }

    // REGISTRAR DEVOLUCIÓN
    public void RegistrarDevolucion(int prestamoId)
    {
        using var conexion = Conectar();
        using var transaccion = conexion.BeginTransaction();

        using var buscar = conexion.CreateCommand();
        buscar.Transaction = transaccion;
        buscar.CommandText = @"
        SELECT LibroId FROM Prestamos
        WHERE Id=$id AND FechaDevolucion IS NULL";
        buscar.Parameters.AddWithValue("$id", prestamoId);

        object? resultado = buscar.ExecuteScalar();

        if (resultado == null)
            throw new Exception(
                "El préstamo no existe o ya fue devuelto.");

        int libroId = Convert.ToInt32(resultado);

        using var devolver = conexion.CreateCommand();
        devolver.Transaction = transaccion;
        devolver.CommandText = @"
        UPDATE Prestamos
        SET FechaDevolucion=$fecha
        WHERE Id=$id";
        devolver.Parameters.AddWithValue("$id", prestamoId);
        devolver.Parameters.AddWithValue(
            "$fecha", DateTime.Today.ToString("yyyy-MM-dd"));
        devolver.ExecuteNonQuery();

        using var sumar = conexion.CreateCommand();
        sumar.Transaction = transaccion;
        sumar.CommandText = @"
        UPDATE Libros SET Ejemplares=Ejemplares+1
        WHERE Id=$id";
        sumar.Parameters.AddWithValue("$id", libroId);
        sumar.ExecuteNonQuery();

        transaccion.Commit();
    }
}
