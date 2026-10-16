# 📚 Sistema de Préstamos de Biblioteca

¡Bienvenido/a al repositorio del **Sistema de Préstamos de Biblioteca**! 👋

Este proyecto fue desarrollado en **C#** como una aplicación de consola que permite administrar lectores, libros y préstamos de una biblioteca, utilizando **SQLite** para almacenar la información de forma persistente.

## 🎯 Descripción del proyecto

El sistema permite gestionar las operaciones básicas de una biblioteca mediante un menú interactivo, facilitando el registro y la consulta de información, así como el control de los préstamos y las devoluciones de libros.

## ✨ Funcionalidades principales

- 👤 Registrar, consultar, actualizar y eliminar lectores.
- 📖 Registrar, consultar, actualizar y eliminar libros.
- 📚 Registrar préstamos de libros.
- 🔎 Consultar los préstamos registrados.
- 🔄 Registrar devoluciones.
- 📦 Controlar la cantidad de ejemplares disponibles.
- 🗃️ Almacenar la información mediante SQLite.
- 🧩 Aplicar conceptos de programación orientada a objetos, como herencia y polimorfismo.

## 🛠️ Tecnologías utilizadas

- **Lenguaje:** C#
- **Plataforma:** .NET 10
- **Base de datos:** SQLite
- **Biblioteca de acceso a datos:** Microsoft.Data.Sqlite
- **Control de versiones:** Git y GitHub
- **Editor de código:** Visual Studio Code

## 📁 Estructura del proyecto

```text
PrestamosBiblioteca/
├── Aplicacion/
│   └── MenuBiblioteca.cs
├── Dominio/
│   ├── Persona.cs
│   ├── Lector.cs
│   ├── Libro.cs
│   └── Prestamo.cs
├── Persistencia/
│   └── RepositorioBiblioteca.cs
├── Program.cs
├── PrestamosBiblioteca.csproj
├── .gitignore
└── README.md
```

## 🚀 Cómo ejecutar el proyecto

1. Instala el SDK de .NET 10.
2. Clona este repositorio:

   ```bash
   git clone https://github.com/jrodriguezj2-dev/PrestamosBiblioteca.git
   ```

3. Entra en la carpeta del proyecto:

   ```bash
   cd PrestamosBiblioteca
   ```

4. Ejecuta la aplicación:

   ```bash
   dotnet run
   ```

5. Utiliza el menú de opciones para administrar los lectores, libros y préstamos.

> **Nota:** La base de datos SQLite se crea y prepara automáticamente al iniciar la aplicación. Los archivos locales de base de datos están excluidos del repositorio mediante `.gitignore`.

## 🎓 Propósito académico

Este proyecto permite poner en práctica conocimientos de programación en C#, programación orientada a objetos, persistencia de datos, operaciones CRUD y manejo de bases de datos relacionales.

¡Gracias por visitar este repositorio! ⭐
