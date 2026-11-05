# Ejecutar Avalonia en navegador con Rider (WebAssembly)

Una aplicación **Avalonia Cross Platform Application** puede funcionar también en un navegador web gracias a **WebAssembly (WASM)**. En esta guía usamos **Windows, .NET 10 y JetBrains Rider**.

!!! info "Requisitos previos"
    Debes tener instalado el **.NET SDK 10**, las **plantillas Avalonia** y una solución creada con la plantilla **Avalonia Cross Platform Application**. Puedes seguir primero la [guía de preparación de Avalonia](instalacion-avalonia-android.md), apartados 1 a 4. Para ejecutar únicamente el proyecto Browser **no hace falta** instalar JDK, Android SDK ni emulador.

En los ejemplos usamos `AvaloniaApplication1`; **sustitúyelo por el nombre de tu aplicación**.

## 1. Elegir Microsoft Edge en Rider

Este ajuste resulta útil si Rider abre el navegador equivocado o no localiza el ejecutable.

1. Abrir **Rider** y pulsar **Ctrl + Alt + S**.
2. Ir a **Tools → Web Browsers and Preview**.
3. Asegurarse de que **Microsoft Edge** está habilitado en la lista.
4. En **Default Browser**, escoger explícitamente Edge cuando la versión lo permita o usar **Custom path** con la ubicación de `msedge.exe`.
5. Guardar los cambios.

Una ubicación habitual en Windows es:

```text
C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe
```

!!! note
    No es obligatorio cambiar el navegador predeterminado para compilar Avalonia Browser: este apartado sirve para **controlar qué navegador abre Rider**. Si Edge ya funciona, podemos pasar al siguiente paso.

## 2. Instalar las herramientas WebAssembly

Desde PowerShell:

```powershell
dotnet workload install wasm-tools
```

`wasm-tools` incorpora las herramientas necesarias para compilar la aplicación .NET para WebAssembly. **No equivale** a la carga `android`.

Comprobar las cargas instaladas:

```powershell
dotnet workload list
```

## 3. Preparar el certificado HTTPS de desarrollo

Normalmente basta con confiar en el certificado de desarrollo:

```powershell
dotnet dev-certs https --trust
```

Si Edge muestra errores de certificado **y el comando anterior no lo soluciona**, cerrar Rider y Edge y regenerar el certificado:

```powershell
dotnet dev-certs https --clean
dotnet dev-certs https --trust
```

!!! warning "`--clean` es una operación de reparación"
    **Elimina los certificados HTTPS de desarrollo de .NET del usuario.** Puede afectar a otros proyectos locales hasta que vuelvan a confiar en su certificado. No hay que ejecutarlo sistemáticamente en todos los equipos si HTTPS ya funciona.

Tras hacerlo, **volver a abrir Rider y Edge**.

## 4. Limpiar, restaurar y compilar el proyecto Browser

Abrir PowerShell en la carpeta donde está la solución.

### Limpiar compilaciones anteriores

```powershell
dotnet clean .\AvaloniaApplication1.sln
```

Elimina los resultados de compilación anteriores; es útil para resolver errores, pero **no es obligatorio cada vez**.

### Restaurar dependencias NuGet

```powershell
dotnet restore .\AvaloniaApplication1.sln
```

Descarga o comprueba las bibliotecas que usan los proyectos.

### Compilar solamente el proyecto del navegador

```powershell
dotnet build .\AvaloniaApplication1.Browser\AvaloniaApplication1.Browser.csproj
```

Esto comprueba la **compilación** del proyecto Browser. **Compilar no es lo mismo que ejecutar**.

!!! tip "Diferencia entre comandos"
    - `dotnet workload restore`: instala las **cargas de trabajo** de .NET que requiere la solución.
    - `dotnet restore`: restaura los **paquetes NuGet**.
    - `dotnet clean`: limpia resultados de compilaciones previas.
    - `dotnet build`: compila el proyecto.

## 5. Ejecutar la aplicación en Edge

Desde la carpeta de la solución:

```powershell
dotnet run --project .\AvaloniaApplication1.Browser\AvaloniaApplication1.Browser.csproj
```

La terminal mostrará una o varias direcciones locales (`http://...` o `https://...`). Abrir una de ellas en **Edge**.

También podemos configurar en **Rider** la ejecución del proyecto `AvaloniaApplication1.Browser` y utilizar **Run**.

!!! success "Resultado esperado"
    Veremos la interfaz Avalonia en una pestaña de Edge. La aplicación se ejecuta como **WebAssembly dentro del navegador**, no como aplicación Android ni como programa de escritorio.

## Errores habituales

| Problema | Qué comprobar |
| --- | --- |
| `winget` no se reconoce | Instalar App Installer o usar el instalador web de .NET. |
| `dotnet` no se reconoce después de instalarlo | Cerrar y abrir la terminal; comprobar `dotnet --version`. |
| Falta una carga WebAssembly | Ejecutar `dotnet workload install wasm-tools`. |
| Rider abre otro navegador | Revisar **Tools → Web Browsers and Preview**. |
| Aviso de certificado HTTPS | Probar `dotnet dev-certs https --trust`; reservar `--clean` para la reparación. |
| Error de compilación tras instalar herramientas | Recargar proyectos de Rider; ejecutar `dotnet restore` y `dotnet build`. |
| La página no se abre | Revisar la URL y el puerto que muestra `dotnet run`; dejar la aplicación en ejecución. |

### Referencias

- [Avalonia: WebAssembly](https://docs.avaloniaui.net/docs/platform-specific-guides/webassembly)
- [JetBrains: configuración de navegadores](https://www.jetbrains.com/help/rider/Configuring_Browsers.html)
- [Microsoft: certificados de desarrollo .NET](https://learn.microsoft.com/dotnet/core/tools/dotnet-dev-certs)
- [Volver a Avalonia en Android](instalacion-avalonia-android.md)
