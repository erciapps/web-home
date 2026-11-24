# Práctica PMDM · DinoManager: SQLite y PowerSync (parte 1)

**Módulo:** Programación Multimedia y Dispositivos Móviles (PMDM)  
**Curso:** 2026-2027  
**Tecnologías:** C# · Avalonia UI · MVVM · Entity Framework Core · SQLite · PostgreSQL · PowerSync

## 1. Objetivo

Crear una aplicación **DinoManager** capaz de gestionar dinosaurios con **SQLite local** en Windows y Android. Después, preparar **PostgreSQL** y **PowerSync** para descargar datos desde el servidor.

!!! info "Dos etapas"
    **En esta primera parte:** operaciones CRUD locales y descarga **PostgreSQL → SQLite**. Se trabajará temporalmente con **dos bases SQLite diferentes**.

    **Objetivo final:** una única base SQLite local gestionada por PowerSync, con cambios en ambos sentidos, una API de escritura y sincronización al recuperar Internet. **No se consigue todavía en esta primera parte.**

### Esquema de lo que se hará ahora

~~~mermaid
flowchart TD
    A["Aplicación Avalonia · Android/Windows"]
    B["SQLite · dinosaurios.db"]
    C["PostgreSQL · tabla Dinosaurios"]
    D["PowerSync · Docker"]
    E["SQLite · powersync.db"]
    A -->|"CRUD local"| B
    C -->|"Replicación lógica"| D
    D -->|"Descarga"| E
    A -->|"Consulta"| E
~~~

Los métodos CRUD trabajan sobre `dinosaurios.db`. PowerSync mantiene por separado `powersync.db` para mostrar los registros descargados.

### Esquema de lo que se pretende conseguir al finalizar el proyecto

~~~mermaid
flowchart LR
    A["Avalonia · Android/Windows"]
    B["SQLite local · PowerSync"]
    C["PowerSync"]
    D["PostgreSQL"]
    E["API de escritura"]
    A <-->|"CRUD sin conexión"| B
    D -->|"Cambios remotos"| C
    C -->|"Descarga"| B
    B -->|"Cambios pendientes"| E
    E -->|"Insertar / modificar / eliminar"| D
~~~

Al finalizar, las modificaciones hechas desde un móvil deberán llegar a PostgreSQL y al resto de dispositivos, incluso si se realizaron inicialmente sin conexión.

## 2. Crear la base SQLite local

### Paso 1. Preparar el proyecto

Crear un proyecto Avalonia con soporte para Android y abrir el **proyecto principal**.

Instalar únicamente el paquete necesario para SQLite con Entity Framework Core:

~~~powershell
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
~~~

**No instalar `CommunityToolkit.Mvvm`:** ya está incluido en el proyecto inicial. Comprobar también que el proyecto tiene disponible `DataGrid` de Avalonia.

### Paso 2. Crear el modelo `Dinosaurio`

Crear la carpeta `Models` y la clase `Dinosaurio.cs`:

~~~csharp
using System;

namespace AndroidExample.Models;

public class Dinosaurio
{
    // Identificador único generado al crear cada dinosaurio
    public string Id { get; set; } = Guid.NewGuid().ToString();

    // Datos almacenados de cada dinosaurio
    public string Nombre { get; set; } = "";
    public string Especie { get; set; } = "";
    public double Peso { get; set; }
}
~~~

Cada objeto representa un dinosaurio. `Guid.NewGuid()` genera un identificador único para que los registros puedan identificarse también al sincronizarlos.

### Paso 3. Crear `AppDbContext`

Crear `Service/AppDbContext.cs`:

~~~csharp
using System;
using System.IO;
using AndroidExample.Models;
using Microsoft.EntityFrameworkCore;

namespace AndroidExample.Service;

public class AppDbContext : DbContext
{
    // Tabla de dinosaurios gestionada por Entity Framework
    public DbSet<Dinosaurio> Dinosaurios { get; set; } = null!;

    protected override void OnConfiguring(
        DbContextOptionsBuilder optionsBuilder)
    {
        // Carpeta privada de la aplicación (Windows y Android)
        string carpeta = Environment.GetFolderPath(
            Environment.SpecialFolder.LocalApplicationData);

        // Crear la carpeta si todavía no existe
        Directory.CreateDirectory(carpeta);

        // Ruta del archivo SQLite local
        string ruta = Path.Combine(carpeta, "dinosaurios.db");

        // Utilizar SQLite como proveedor de Entity Framework
        optionsBuilder.UseSqlite($"Data Source={ruta}");
    }
}
~~~

`AppDbContext` permite acceder a SQLite utilizando objetos C# en lugar de escribir las operaciones SQL manualmente. La base se guarda en una carpeta con permisos de escritura para la aplicación, también en Android.

### Paso 4. Crear `SQLiteService`

Crear `Service/SQLiteService.cs`:

~~~csharp
using System.Collections.Generic;
using System.Linq;
using AndroidExample.Models;
using Avalonia.Collections;

namespace AndroidExample.Service;

public class SQLiteService
{
    // Crear la base de datos y sus tablas si no existen
    public SQLiteService()
    {
        using var db = new AppDbContext();
        db.Database.EnsureCreated();
    }

    // Insertar un dinosaurio en SQLite
    public void Insertar(Dinosaurio dinosaurio)
    {
        using var db = new AppDbContext();

        db.Dinosaurios.Add(dinosaurio);
        db.SaveChanges();
    }

    // Recuperar todos los dinosaurios como colección de Avalonia
    public AvaloniaList<Dinosaurio> ObtenerTodos()
    {
        using var db = new AppDbContext();

        return new AvaloniaList<Dinosaurio>(db.Dinosaurios.ToList());
    }

    // Modificar un dinosaurio existente
    public void Actualizar(Dinosaurio dinosaurio)
    {
        using var db = new AppDbContext();

        db.Dinosaurios.Update(dinosaurio);
        db.SaveChanges();
    }

    // Eliminar un dinosaurio
    public void Eliminar(Dinosaurio dinosaurio)
    {
        using var db = new AppDbContext();

        db.Dinosaurios.Remove(dinosaurio);
        db.SaveChanges();
    }
}
~~~

| Método | Para qué se utiliza |
| --- | --- |
| Constructor `SQLiteService()` | Crear la base y las tablas si todavía no existen |
| `Insertar()` | Añadir un dinosaurio con `Add()` |
| `ObtenerTodos()` | Obtener los registros y devolver una `AvaloniaList` |
| `Actualizar()` | Modificar un dinosaurio con `Update()` |
| `Eliminar()` | Quitar un dinosaurio con `Remove()` |

Cada método utiliza `using var` para liberar el contexto al terminar. `SaveChanges()` confirma las operaciones de escritura.

## 3. Crear la interfaz Avalonia

### Paso 5. Preparar `ConfiguracionView.axaml`

Crear una vista `ContentPage` que contenga un formulario, un listado local y un listado remoto.

**Todavía no se deben añadir `Binding` ni `Command`.** Se implementarán en clase, junto con los métodos del ViewModel, para evitar errores antes de disponer de las propiedades y comandos necesarios.

El siguiente código es el **contenido interior** de la vista `ContentPage` existente. Conservar la cabecera, los espacios de nombres y el `x:Class` que genere el proyecto.

~~~xml
<!-- COLORES PARA TEMA CLARO Y OSCURO -->
<ContentPage.Resources>
    <ResourceDictionary>
        <ResourceDictionary.ThemeDictionaries>
            <ResourceDictionary x:Key="Light">
                <SolidColorBrush x:Key="PageBg" Color="#F3F5F9"/>
                <SolidColorBrush x:Key="CardBg" Color="#FFFFFF"/>
                <SolidColorBrush x:Key="CardBorder" Color="#E2E8F0"/>
                <SolidColorBrush x:Key="MainText" Color="#17243A"/>
                <SolidColorBrush x:Key="MutedText" Color="#64748B"/>
                <SolidColorBrush x:Key="AccentBg" Color="#15803D"/>
                <SolidColorBrush x:Key="AccentText" Color="#FFFFFF"/>
            </ResourceDictionary>
            <ResourceDictionary x:Key="Dark">
                <SolidColorBrush x:Key="PageBg" Color="#101318"/>
                <SolidColorBrush x:Key="CardBg" Color="#1C222B"/>
                <SolidColorBrush x:Key="CardBorder" Color="#333B48"/>
                <SolidColorBrush x:Key="MainText" Color="#F1F5F9"/>
                <SolidColorBrush x:Key="MutedText" Color="#94A3B8"/>
                <SolidColorBrush x:Key="AccentBg" Color="#166534"/>
                <SolidColorBrush x:Key="AccentText" Color="#FFFFFF"/>
            </ResourceDictionary>
        </ResourceDictionary.ThemeDictionaries>
    </ResourceDictionary>
</ContentPage.Resources>

<ScrollViewer Background="{DynamicResource PageBg}">
    <StackPanel Margin="14" Spacing="14">

        <!-- CABECERA -->
        <Border Background="{DynamicResource CardBg}"
                BorderBrush="{DynamicResource CardBorder}"
                BorderThickness="1"
                CornerRadius="14"
                Padding="18">
            <StackPanel Spacing="5">
                <TextBlock Text="🦖 DinoManager"
                           FontSize="26"
                           FontWeight="Bold"
                           Foreground="{DynamicResource MainText}"/>
                <TextBlock Text="Gestión de dinosaurios"
                           FontSize="14"
                           Foreground="{DynamicResource MutedText}"/>
                <TextBlock Text="SQLite · Entity Framework Core"
                           FontSize="12"
                           Foreground="{DynamicResource MutedText}"/>
            </StackPanel>
        </Border>

        <!-- FORMULARIO -->
        <Border Background="{DynamicResource CardBg}"
                BorderBrush="{DynamicResource CardBorder}"
                BorderThickness="1"
                CornerRadius="14"
                Padding="16">
            <StackPanel Spacing="10">
                <TextBlock Text="Datos del dinosaurio"
                           FontSize="19"
                           FontWeight="SemiBold"
                           Foreground="{DynamicResource MainText}"/>
                <TextBlock Text="Nombre"
                           Foreground="{DynamicResource MainText}"/>
                <TextBox Watermark="Ej. Rex"/>
                <TextBlock Text="Especie"
                           Foreground="{DynamicResource MainText}"/>
                <TextBox Watermark="Ej. Tiranosaurio"/>
                <TextBlock Text="Peso (kg)"
                           Foreground="{DynamicResource MainText}"/>
                <TextBox Watermark="Ej. 7000"/>

                <!-- COMMANDS: se añadirán en clase -->
                <Button Content="Guardar dinosaurio"
                        Background="{DynamicResource AccentBg}"
                        Foreground="{DynamicResource AccentText}"
                        HorizontalAlignment="Stretch"
                        HorizontalContentAlignment="Center"
                        Margin="0,8,0,0"/>
                <Grid ColumnDefinitions="*,*" ColumnSpacing="8">
                    <Button Grid.Column="0"
                            Content="Nuevo"
                            HorizontalAlignment="Stretch"
                            HorizontalContentAlignment="Center"/>
                    <Button Grid.Column="1"
                            Content="Eliminar"
                            HorizontalAlignment="Stretch"
                            HorizontalContentAlignment="Center"/>
                </Grid>

                <!-- Estado de la operación: se enlazará más adelante -->
                <TextBlock Foreground="{DynamicResource MutedText}"
                           TextWrapping="Wrap"/>
            </StackPanel>
        </Border>

        <!-- DINOSAURIOS ALMACENADOS EN SQLITE LOCAL -->
        <Border Background="{DynamicResource CardBg}"
                BorderBrush="{DynamicResource CardBorder}"
                BorderThickness="1"
                CornerRadius="14"
                Padding="16">
            <StackPanel Spacing="12">
                <TextBlock Text="Dinosaurios registrados"
                           FontSize="19"
                           FontWeight="SemiBold"
                           Foreground="{DynamicResource MainText}"/>
                <TextBlock Text="Selecciona uno para editarlo."
                           FontSize="12"
                           Foreground="{DynamicResource MutedText}"/>

                <!-- ItemsSource y SelectedItem: se añadirán en clase -->
                <DataGrid AutoGenerateColumns="True"
                          IsReadOnly="True"
                          Height="240"
                          RowHeight="42"
                          HeadersVisibility="Column"/>

                <Button Content="Actualizar listado"
                        HorizontalAlignment="Stretch"
                        HorizontalContentAlignment="Center"/>
            </StackPanel>
        </Border>

        <!-- DINOSAURIOS DESCARGADOS DESDE POSTGRESQL -->
        <Border Background="{DynamicResource CardBg}"
                BorderBrush="{DynamicResource CardBorder}"
                BorderThickness="1"
                CornerRadius="14"
                Padding="16">
            <StackPanel Spacing="12">
                <TextBlock Text="Dinosaurios de PostgreSQL"
                           FontSize="19"
                           FontWeight="SemiBold"
                           Foreground="{DynamicResource MainText}"/>
                <Button Content="Sincronizar con PostgreSQL"
                        HorizontalAlignment="Stretch"
                        HorizontalContentAlignment="Center"/>
                <!-- ItemsSource remoto: se añadirá en clase -->
                <DataGrid AutoGenerateColumns="True"
                          IsReadOnly="True"
                          Height="200"/>
            </StackPanel>
        </Border>
    </StackPanel>
</ScrollViewer>
~~~

### Paso 6. ViewModel (se realizará en clase)

Se utilizará `MainViewModel` siguiendo el patrón MVVM. Se incorporarán las propiedades `Formulario`, `Dinosaurios`, `DinosaurioSeleccionado` y `Estado`; los comandos `GuardarCommand`, `NuevoCommand`, `EliminarCommand` y `CargarCommand`; y posteriormente `DinosauriosRemotos` y `SincronizarCommand`.

**Los métodos del ViewModel y los bindings se desarrollarán en clase**, por lo que los botones y las tablas todavía no estarán conectados al servicio.

## 4. Preparar PowerSync en Portainer

### Paso 7. Elegir puertos disponibles

Cada alumno deberá seleccionar **los puertos que tenga libres y asignados en su entorno**. No se puede publicar el mismo puerto dos veces en el mismo servidor Docker.

| Servicio | Puerto publicado de ejemplo | Qué debe hacer el alumno |
| --- | --- | --- |
| PostgreSQL | `10001` | Usar el puerto de su PostgreSQL |
| PowerSync | `10002` | Elegir otro puerto libre |
| Generador de tokens | `10003` | Elegir un tercer puerto libre |

En `"10002:80"`, el puerto **10002 es el publicado**, que debe adaptarse; **80 es el interno**, que se mantiene. En `"10003:3000"` ocurre lo mismo con `3000`.

!!! warning "Instancias compartidas"
    Si varios alumnos comparten servidor PostgreSQL, el profesor deberá organizar las bases de datos y permisos para evitar que todos utilicen la misma tabla o el mismo almacenamiento interno por accidente. La configuración global `wal_level` solo se realiza una vez por instancia.

### Paso 8. Preparar un stack en Portainer

Entrar en **Portainer → Stacks → Add stack → Web editor**. Crear un stack denominado, por ejemplo, `powersync-dino`.

Sustituir **usuario, contraseña, IP, puerto PostgreSQL, puertos libres y clave JWT**. La configuración del stream utiliza el esquema `dinosaruios`, tal como está creado en PostgreSQL.

~~~yaml
# Clave compartida para los JWT de prueba
x-jwt-key: &jwt-key "CLAVE_BASE64URL_DE_PRUEBA"

services:
  # SERVICIO DE SINCRONIZACIÓN
  powersync:
    image: journeyapps/powersync-service:1.29.1
    restart: unless-stopped
    ports:
      # Cambiar 10002 por un puerto disponible
      - "10002:80"
    environment:
      POWERSYNC_CONFIG_PATH: /config/service.yaml

      # PostgreSQL con la tabla de dinosaurios
      PS_DATA_SOURCE_URI: "postgresql://USUARIO:CLAVE@IP_POSTGRES:PUERTO_POSTGRES/dam"

      # Base de datos interna de PowerSync
      PS_STORAGE_SOURCE_URI: "postgresql://USUARIO:CLAVE@IP_POSTGRES:PUERTO_POSTGRES/powersync_storage"

      PS_CLIENT_AUTH_KEY: *jwt-key
    configs:
      - source: servicio
        target: /config/service.yaml

  # GENERADOR DE TOKENS (SOLO PARA EL LABORATORIO)
  token:
    image: node:22-alpine
    restart: unless-stopped
    ports:
      # Cambiar 10003 por otro puerto disponible
      - "10003:3000"
    environment:
      PS_CLIENT_AUTH_KEY: *jwt-key
    command:
      - node
      - -e
      - |
        const http = require("node:http");
        const crypto = require("node:crypto");

        // Convertir datos a Base64URL
        const encode = obj =>
          Buffer.from(JSON.stringify(obj)).toString("base64url");

        http.createServer((req, res) => {
          // Solo atender GET /token
          if (req.url !== "/token" || req.method !== "GET") {
            res.writeHead(404);
            return res.end();
          }

          // Fecha actual y caducidad de una hora
          const now = Math.floor(Date.now() / 1000);

          const header = encode({
            alg: "HS256",
            typ: "JWT",
            kid: "dev-key-1"
          });

          const payload = encode({
            sub: "dinos-test",
            aud: "powersync",
            iat: now,
            exp: now + 3600
          });

          // Firma del JWT con la misma clave que PowerSync
          const data = header + "." + payload;
          const signature = crypto
            .createHmac(
              "sha256",
              Buffer.from(process.env.PS_CLIENT_AUTH_KEY, "base64url"))
            .update(data)
            .digest("base64url");

          // Devolver el token al cliente
          res.writeHead(200, {
            "Content-Type": "text/plain",
            "Cache-Control": "no-store"
          });
          res.end(data + "." + signature);
        }).listen(3000, "0.0.0.0");

# CONFIGURACIÓN DEL SERVICIO POWERSYNC
configs:
  servicio:
    content: |
      _type: self-hosted

      # Conexión a la base con los datos
      replication:
        connections:
          - type: postgresql
            uri: !env PS_DATA_SOURCE_URI
            sslmode: disable

      # Almacenamiento de los datos internos de PowerSync
      storage:
        type: postgresql
        uri: !env PS_STORAGE_SOURCE_URI
        sslmode: disable

      port: 80

      # Tabla que queremos descargar en los dispositivos
      sync_config:
        exit_on_error: true
        content: |
          config:
            edition: 3

          streams:
            dinosaurios:
              auto_subscribe: true
              query: SELECT "Id" AS id, "Nombre", "Especie", "Peso" FROM dinosaruios."Dinosaurios"

      # Validación de los tokens
      client_auth:
        audience: ["powersync"]
        jwks:
          keys:
            - kty: oct
              alg: HS256
              kid: dev-key-1
              k: !env PS_CLIENT_AUTH_KEY
~~~

La clave `CLAVE_BASE64URL_DE_PRUEBA` debe reemplazarse por una clave aleatoria Base64URL de 32 bytes. El profesor puede generarla en PowerShell:

~~~powershell
[Convert]::ToBase64String(
    [System.Security.Cryptography.RandomNumberGenerator]::GetBytes(32)
).TrimEnd('=').Replace('+','-').Replace('/','_')
~~~

La clave debe ser la misma para PowerSync y el generador de tokens. **No publicar contraseñas reales en Git.**

!!! warning "Servidor de tokens de laboratorio"
    Este endpoint entrega tokens a cualquiera que pueda alcanzarlo; **no identifica al alumno**. Utilizar solamente en una red de prácticas aislada, con el acceso restringido. No publicarlo en Internet. La autenticación real se incorporará en una ampliación.

Antes de desplegar el stack, completar el SQL del siguiente apartado.

## 5. Preparar PostgreSQL

### Paso 9. Activar la replicación lógica

PowerSync lee los cambios de PostgreSQL mediante **replicación lógica**. Para permitirla, abrir pgAdmin con una cuenta administradora y ejecutar:

~~~sql
-- Permitir que PostgreSQL envíe los cambios de sus tablas
ALTER SYSTEM SET wal_level = logical;
~~~

**Reiniciar el PostgreSQL correspondiente** desde Portainer para aplicar el cambio y comprobar:

~~~sql
-- Debe devolver: logical
SHOW wal_level;

-- Deben ser mayores que cero
SHOW max_replication_slots;
SHOW max_wal_senders;
~~~

Este cambio afecta a toda la instancia PostgreSQL y solo se necesita una vez. Si ya devuelve `logical`, no repetirlo.

### Paso 10. Crear la base de almacenamiento interno

Conectarse a una base administrativa (por ejemplo, `postgres`) y ejecutar una sola vez, como sentencia independiente:

~~~sql
-- PowerSync utiliza esta base para su estado interno
CREATE DATABASE powersync_storage;
~~~

No crear manualmente tablas en esta base: las administrará PowerSync. Si ya existe, no volver a ejecutar `CREATE DATABASE`.

### Paso 11. Crear la tabla de dinosaurios

Cambiar en pgAdmin a la base de datos `dam` y ejecutar:

~~~sql
-- Esquema donde estarán los dinosaurios
CREATE SCHEMA IF NOT EXISTS dinosaruios;

-- Tabla equivalente al modelo Dinosaurio de C#
CREATE TABLE IF NOT EXISTS dinosaruios."Dinosaurios"
(
    "Id" TEXT PRIMARY KEY,                -- Identificador único
    "Nombre" TEXT NOT NULL,               -- Nombre
    "Especie" TEXT NOT NULL,              -- Especie
    "Peso" DOUBLE PRECISION NOT NULL      -- Peso en kg
);
~~~

### Paso 12. Insertar datos de prueba

~~~sql
-- Primer dinosaurio del servidor
INSERT INTO dinosaruios."Dinosaurios"
("Id", "Nombre", "Especie", "Peso")
VALUES
(gen_random_uuid()::text, 'Rex', 'Tiranosaurio', 7000);

-- Comprobar el contenido
SELECT *
FROM dinosaruios."Dinosaurios";
~~~

### Paso 13. Crear la publicación

La publicación indica qué tabla debe formar parte de la replicación lógica:

~~~sql
-- Ejecutar solo si todavía no existe la publicación
CREATE PUBLICATION powersync
FOR TABLE dinosaruios."Dinosaurios";
~~~

Si `powersync` ya existe pero no contiene esta tabla, ejecutar **en su lugar**:

~~~sql
ALTER PUBLICATION powersync
ADD TABLE dinosaruios."Dinosaurios";
~~~

Comprobar que la tabla aparece publicada:

~~~sql
SELECT pubname, schemaname, tablename
FROM pg_publication_tables
WHERE pubname = 'powersync';
~~~

| pubname | schemaname | tablename |
| --- | --- | --- |
| powersync | dinosaruios | Dinosaurios |

### Paso 14. Arrancar el stack y comprobar

Desplegar o actualizar el stack desde Portainer. Revisar los logs para detectar errores de conexión, permisos, configuración o replicación.

Comprobar desde el navegador, sustituyendo los datos por los de cada alumno:

~~~text
http://IP_SERVIDOR:PUERTO_POWERSYNC/probes/liveness
http://IP_SERVIDOR:PUERTO_TOKEN/token
~~~

La primera URL verifica que el servicio web responde; **no confirma por sí sola que la replicación esté funcionando**. La segunda devuelve un JWT temporal, que **no habrá que copiar** porque Avalonia lo obtendrá automáticamente.

## 6. Descargar los datos en Avalonia

### Paso 15. Instalar PowerSync

En el proyecto principal:

~~~powershell
dotnet add package PowerSync.Common --version 1.0.1
~~~

### Paso 16. Preparar Android

En `AndroidExample.Android/Properties/AndroidManifest.xml` comprobar que existe el permiso:

~~~xml
<!-- Permitir el acceso a Internet -->
<uses-permission android:name="android.permission.INTERNET" />
~~~

Dentro del elemento `<application>` existente añadir el atributo:

~~~xml
android:usesCleartextTraffic="true"
~~~

El atributo permite usar **HTTP durante esta práctica de laboratorio**. Para una aplicación pública se deberá utilizar HTTPS.

### Paso 17. Crear `SincronizacionService`

En la carpeta `Service`, crear `SincronizacionService.cs`:

~~~csharp
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using AndroidExample.Models;
using PowerSync.Common.Client;
using PowerSync.Common.Client.Connection;
using PowerSync.Common.DB.Schema;

namespace AndroidExample.Service;

public class SincronizacionService
{
    // Base SQLite gestionada por PowerSync
    private PowerSyncDatabase? db;

    public async Task<List<Dinosaurio>> Descargar()
    {
        // Preparar la conexión solamente la primera vez
        if (db == null)
        {
            // Carpeta privada de Windows o Android
            string carpeta = Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData);

            Directory.CreateDirectory(carpeta);

            // Archivo local donde PowerSync guarda sus datos
            string ruta = Path.Combine(carpeta, "powersync.db");

            // Crear la base y definir las columnas que se descargan
            db = new PowerSyncDatabase(new PowerSyncDatabaseOptions
            {
                Database = new SQLOpenOptions
                {
                    DbFilename = ruta
                },
                Schema = new Schema(
                    new Table
                    {
                        Name = "Dinosaurios",
                        Columns =
                        {
                            ["Nombre"] = ColumnType.Text,
                            ["Especie"] = ColumnType.Text,
                            ["Peso"] = ColumnType.Real
                        }
                    })
            });

            // Inicializar el archivo SQLite
            await db.Init();

            // Conectar con el servidor del alumno
            // Reemplazar IP_SERVIDOR y PUERTO_POWERSYNC
            await db.Connect(
                "http://IP_SERVIDOR:PUERTO_POWERSYNC",
                new AutenticacionPowerSync(),
                null);
        }

        // Esperar a que finalice la primera descarga
        try
        {
            await db.WaitForFirstSync()
                .WaitAsync(TimeSpan.FromSeconds(30));
        }
        catch (TimeoutException)
        {
            // Mostrar información útil si se agota la espera
            var estado = db.CurrentStatus;

            throw new Exception(
                $"Conectado: {estado.Connected}\n" +
                $"Conectando: {estado.Connecting}\n" +
                $"Sincronizado: {estado.HasSynced}\n" +
                $"Error descarga: {estado.DataFlowStatus.DownloadError?.Message ?? "Ninguno"}");
        }

        // Convertir las filas descargadas en objetos Dinosaurio
        return await db.GetAll(
            "SELECT id, Nombre, Especie, Peso FROM Dinosaurios",
            null,
            fila => new Dinosaurio
            {
                Id = fila.GetString("id"),
                Nombre = fila.GetString("Nombre"),
                Especie = fila.GetString("Especie"),
                Peso = fila.GetDouble("Peso")
            });
    }
}

// Obtener automáticamente el token de acceso
public class AutenticacionPowerSync : IAuthenticator
{
    public async Task<string> ResolveCredentials()
    {
        using HttpClient http = new HttpClient();

        // Sustituir la IP y el puerto del generador de tokens
        return await http.GetStringAsync(
            "http://IP_SERVIDOR:PUERTO_TOKEN/token");
    }
}
~~~

| Elemento | Para qué sirve |
| --- | --- |
| `powersync.db` | Archivo SQLite gestionado por PowerSync |
| `Schema` | Columnas que tendrá la tabla local |
| `db.Init()` | Inicializar la base de datos |
| `db.Connect()` | Conectar con PowerSync |
| `ResolveCredentials()` | Obtener un JWT automáticamente |
| `WaitForFirstSync()` | Esperar a la primera sincronización |
| `GetAll()` | Convertir las filas descargadas en objetos C# |

El tercer parámetro de `db.Connect()` es `null` porque **todavía no se ha implementado la subida de modificaciones**.

### Paso 18. Completar el ViewModel en clase

Se añadirá `SincronizarCommand`, que llamará a `Descargar()` y actualizará la colección `DinosauriosRemotos`. Después se enlazará esta colección al segundo `DataGrid` de la vista.

Los **métodos y bindings del ViewModel no se proporcionan resueltos**: se desarrollarán en clase.

## 7. Comprobación de la práctica

Cuando se hayan realizado los métodos del ViewModel, verificar:

| Prueba | Resultado esperado |
| --- | --- |
| Insertar un dinosaurio desde Avalonia | Se guarda en `dinosaurios.db` |
| Cerrar y abrir de nuevo la aplicación | Los registros locales se conservan |
| Modificar o eliminar un dinosaurio | Los cambios se reflejan en SQLite |
| Insertar un dinosaurio desde pgAdmin | Aparece en PostgreSQL |
| Pulsar **Sincronizar** | Se descargan los registros de PostgreSQL |
| Consultar la lista remota | Se muestran los dinosaurios de `powersync.db` |
| Abrir la aplicación sin Internet | El CRUD local sigue siendo accesible |

!!! note "Limitación de esta primera parte"
    Los registros del CRUD local todavía **no se envían a PostgreSQL**, y los registros descargados están en una **segunda base SQLite**. En siguientes prácticas se unificará el almacenamiento y se implementará el envío de cambios.

## 8. Próximas ampliaciones

1. Implementar y completar los comandos y bindings MVVM.
2. Integrar el CRUD con la base local de PowerSync.
3. Crear una API que reciba inserciones, modificaciones y eliminaciones.
4. Configurar la subida de cambios pendientes y la sincronización automática.
5. Comprobar que los cambios hechos sin conexión se envían al recuperar Internet.
