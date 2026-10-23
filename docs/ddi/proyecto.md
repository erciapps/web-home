# Proyecto DDI · Aplicación de gestión con Avalonia

**Módulo:** Desarrollo de Interfaces · 2.º DAM  
**Curso:** 2026-2027  
**Trabajo:** Por parejas

## 1. En qué consiste

Por parejas, desarrollaréis una aplicación de escritorio con **C#, Avalonia UI y XAML** para gestionar los datos de un negocio, servicio o actividad.

Podéis elegir la temática: una tienda, biblioteca, gimnasio, almacén, colección de videojuegos, sistema de reservas, etc. Antes de empezar, comprobad que vuestra propuesta permite cumplir todos los requisitos del proyecto.

La aplicación tendrá varias páginas, formularios para introducir datos, un apartado de consultas y una base de datos donde se guardará la información.

## 2. Tecnologías

| Tecnología | Uso |
| --- | --- |
| C# y .NET | Desarrollo de la aplicación |
| Avalonia UI y XAML | Diseño de las pantallas |
| MVVM | Separación de vistas, datos y lógica de presentación |
| PostgreSQL | Base de datos |
| n8n | Servicios HTTP para consultar y modificar datos |
| Dropbox | Almacenamiento de imágenes |
| SukiUI | Componentes y estilos (opcional) |

La aplicación accederá a PostgreSQL **a través de servicios HTTP creados en n8n**. No se realizarán consultas directas a PostgreSQL desde Avalonia.

## 3. Requisitos del proyecto

### R1. Páginas y navegación

La aplicación tendrá una ventana principal con un menú lateral o superior que permita cambiar entre distintas páginas o vistas.

Como mínimo, incluirá:

- **Bienvenida:** página de inicio con el nombre de la aplicación y acceso a los apartados principales.
- **Altas:** formulario para crear nuevos registros.
- **Consultas:** listado de registros, búsqueda y acceso a sus datos para modificarlos o eliminarlos.

Podéis añadir otras páginas si la aplicación las necesita. La navegación deberá funcionar desde el menú y mantener un diseño coherente entre las vistas.

### R2. MVVM, bindings e interacción con la interfaz

Organizad el proyecto utilizando **MVVM**, con modelos, vistas en XAML y ViewModels.

Se pide utilizar bindings para:

- Mostrar datos del ViewModel en los controles.
- Recoger los valores introducidos en los formularios.
- Ejecutar acciones desde botones mediante comandos.
- Mostrar el registro seleccionado y actualizar la información de la pantalla.
- **Mostrar, ocultar, habilitar o deshabilitar controles según lo que haga el usuario.**

Esto último deberá verse en funcionamiento en al menos una parte del formulario. Por ejemplo, al marcar un `CheckBox` aparece un bloque de campos adicionales; al desmarcarlo, se oculta. También podéis utilizar un desplegable para habilitar o deshabilitar campos relacionados.

La visibilidad y el estado de los controles se gestionarán mediante propiedades y bindings, como `IsVisible` e `IsEnabled`. Evitad concentrar la lógica de la interfaz en el archivo `.axaml.cs`.

### R3. Datos y operaciones CRUD

La base de datos tendrá **al menos tres entidades relacionadas**. Una será la principal y las otras dos servirán para organizar o clasificar sus datos.

Por ejemplo, para un almacén:

- Productos (entidad principal).
- Categorías.
- Proveedores.

La entidad principal tendrá un **CRUD completo**: crear, consultar, modificar y eliminar registros. De las entidades secundarias, como mínimo, se podrán consultar y añadir registros.

Los datos se guardarán en PostgreSQL mediante los servicios de n8n y deberán seguir estando disponibles al cerrar y volver a abrir la aplicación.

### R4. Formularios y desplegables

Utilizad los controles que correspondan a los datos de vuestra aplicación: `TextBox`, `ComboBox`, `CheckBox`, `DatePicker`, `DataGrid`, `ListBox`, botones, etc.

**Será obligatorio disponer de al menos dos desplegables que carguen sus opciones desde PostgreSQL.**

- Sus datos se obtendrán mediante una llamada HTTP a n8n.
- Los elementos se mostrarán mediante `ItemsSource` y binding.
- No se podrán escribir las opciones directamente en XAML ni dejarlas fijas en C#.
- Al guardar, se utilizará el identificador del elemento seleccionado.
- Al modificar un registro, los desplegables mostrarán las opciones que tenía guardadas.

Por ejemplo, en el formulario de productos, los desplegables de categoría y proveedor tendrán que consultar sus tablas de la base de datos.

Los formularios comprobarán los campos obligatorios y mostrarán un mensaje cuando los datos no sean válidos.

### R5. Consultas y búsqueda

La página de consultas permitirá:

- Mostrar los registros de la entidad principal.
- Buscar registros por uno o varios campos.
- Filtrar el listado por algún criterio.
- Seleccionar un registro para consultar sus datos.
- Acceder a la modificación y eliminación del registro seleccionado.

### R6. Varias imágenes por registro

Cada registro de la entidad principal podrá tener **varias imágenes**.

La aplicación deberá permitir:

- Seleccionar una o varias imágenes desde el equipo.
- Subirlas a Dropbox mediante n8n y organizarlas por registro.
- Recuperar las imágenes guardadas para cada registro.
- Mostrarlas en una galería con miniaturas.
- Pulsar una miniatura para ver la imagen ampliada.
- Añadir nuevas imágenes a un registro existente.
- Eliminar una imagen sin borrar las demás.
- Mostrar una imagen predeterminada si el registro no tiene imágenes.

### R7. Diseño y uso de la aplicación

- La interfaz deberá adaptarse a distintos tamaños de ventana.
- Los controles estarán bien distribuidos y serán fáciles de utilizar.
- Se mostrarán mensajes cuando se guarden cambios o se produzca un error.
- Antes de borrar un registro, se pedirá confirmación mediante un diálogo.
- Las llamadas HTTP no deberán bloquear la interfaz.
- La aplicación no deberá cerrarse por un error de conexión o por introducir un dato incorrecto.

## 4. Mejoras opcionales

Podéis añadir funcionalidades extra una vez que estén terminados los requisitos anteriores. Por ejemplo: inicio de sesión, roles, modo claro y oscuro, notificaciones, estadísticas, importación de datos o funciones específicas de vuestra temática.

Estas mejoras no sustituyen los requisitos obligatorios.

## 5. Entrega

El proyecto se entregará en **un repositorio compartido de GitHub** al que tendrán acceso los dos integrantes de la pareja.

El repositorio incluirá:

- El código fuente completo.
- Un `.gitignore` adecuado para .NET y Avalonia.
- Un `README.md` breve con el nombre del proyecto, qué permite hacer y cómo ejecutarlo.
- Capturas de las páginas principales.

Ambos integrantes deberán trabajar con sus propias cuentas de GitHub y realizar commits durante el desarrollo. El historial del repositorio debe permitir ver la participación de cada uno.

No subáis contraseñas, tokens, claves de conexión ni otros datos privados.

## 6. A tener en cuenta

- Los dos integrantes deben conocer el funcionamiento de la aplicación y poder explicar su código.
- No se admitirán formularios que simulen guardar datos sin hacerlo realmente en la base de datos.
- Las listas y los desplegables deberán mostrar información recuperada de los servicios, no datos escritos directamente en el código.
- Se tendrá en cuenta el funcionamiento de la aplicación, la navegación, el uso de bindings y comandos, la organización del proyecto y el diseño de la interfaz.
