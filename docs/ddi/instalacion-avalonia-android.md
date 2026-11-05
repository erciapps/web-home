# Preparar Avalonia para Android con Rider (.NET 10)

Esta guía permite crear y ejecutar en **Windows** una aplicación de **Avalonia Cross Platform Application** en un **emulador Android**, usando **JetBrains Rider**, **.NET 10** y comandos de **PowerShell**.

!!! info "Antes de comenzar"
    - Los comandos se ejecutan en **PowerShell** (terminal de Rider o terminal de Windows).
    - Trabajaremos con el Android SDK en `C:\Android\Sdk`, unificándolo para evitar problemas de rutas.
    - `AvaloniaApplication1` es un **nombre de ejemplo**. Sustitúyelo por el nombre real de tu solución y de tus proyectos.
    - Si ya tienes una herramienta instalada, verifica su versión y pasa al siguiente apartado.

## 1. Instalar .NET SDK 10

Comprobar si ya tenemos el SDK:

```powershell
dotnet --list-sdks
```

Si no aparece una versión `10.0`, instalar desde PowerShell:

```powershell
winget install Microsoft.DotNet.SDK.10 --source winget
```

**Alternativa desde la web:** [descargar .NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0). Hay que instalar el **SDK**, no únicamente el Runtime.

!!! warning "Si `winget` no se reconoce"
    Instala o actualiza **App Installer / Instalador de aplicaciones** desde Microsoft Store. También puedes evitar `winget` y usar el instalador web de .NET.

**Cerrar y volver a abrir la terminal** para que Windows actualice las variables de entorno. Volver a comprobar:

```powershell
dotnet --version
```

## 2. Instalar las plantillas de Avalonia

```powershell
dotnet new install Avalonia.Templates
```

Comprobar que están disponibles:

```powershell
dotnet new list avalonia
```

Las plantillas son los modelos iniciales de proyectos que Rider puede crear.

## 3. Crear una aplicación multiplataforma en Rider

1. Abrir **Rider** y seleccionar **New Solution**.
2. Elegir **Avalonia Cross Platform Application** (no una aplicación solo para escritorio).
3. Indicar un nombre; en los ejemplos usaremos `AvaloniaApplication1`.
4. Crear la solución.

La plantilla genera varios proyectos, normalmente uno compartido, otro de escritorio, uno de Android y otro de navegador (además de otros destinos según la plantilla).

!!! tip "Plugin de Rider"
    En **Settings → Plugins → Marketplace**, instalar **AvaloniaRider** si no está instalado. Aporta, entre otras funciones, vista previa de XAML; **no es el Android SDK ni sustituye las cargas de trabajo de .NET**.

## 4. Restaurar las cargas de trabajo de la solución

Abrir una terminal **en la carpeta que contiene la solución** y ejecutar:

```powershell
dotnet workload restore
```

También podemos señalar expresamente el archivo de solución:

```powershell
dotnet workload restore .\AvaloniaApplication1.sln
```

**No es necesario ejecutar ambos comandos:** hacen la misma operación si estamos en la carpeta correcta. `workload restore` detecta e instala las cargas de trabajo de .NET que necesitan los proyectos.

En Rider, hacer clic derecho sobre la solución y seleccionar **Reload All Projects** para actualizar su estado tras la instalación.

Si solo queremos instalar Android explícitamente, otra opción es:

```powershell
dotnet workload install android
```

Comprobar las cargas instaladas:

```powershell
dotnet workload list
```

## 5. Instalar Microsoft OpenJDK 21

Java es necesario para las herramientas de compilación de Android.

```powershell
winget install Microsoft.OpenJDK.21
```

**Alternativa:** [descargar Microsoft OpenJDK 21 desde la web oficial](https://learn.microsoft.com/es-es/java/openjdk/download).

Cerrar y volver a abrir la terminal si acabamos de instalarlo. Comprobar:

```powershell
java -version
```

### Localizar el JDK (paso imprescindible antes de usar `$jdk`)

En las instrucciones siguientes se utiliza la variable `$jdk`. **Debemos asignarle primero una ruta válida**. Si hemos instalado el JDK de Microsoft en la ubicación habitual:

```powershell
$jdk = Get-ChildItem "C:\Program Files\Microsoft" -Directory -Filter "jdk-21*" |
    Sort-Object Name -Descending |
    Select-Object -First 1 -ExpandProperty FullName

if (-not $jdk) { throw "No se ha encontrado el JDK 21. Revisa su ruta de instalación." }

$jdk
```

Si está instalado en otra carpeta, asigna manualmente esa carpeta a `$jdk` (la que contiene `bin\java.exe`).

### Configurar las variables de entorno

```powershell
[Environment]::SetEnvironmentVariable("JAVA_HOME", $jdk, "User")
[Environment]::SetEnvironmentVariable("ANDROID_HOME", "C:\Android\Sdk", "User")
[Environment]::SetEnvironmentVariable("ANDROID_SDK_ROOT", "C:\Android\Sdk", "User")

$env:JAVA_HOME = $jdk
$env:ANDROID_HOME = "C:\Android\Sdk"
$env:ANDROID_SDK_ROOT = "C:\Android\Sdk"
$env:Path = "$jdk\bin;C:\Android\Sdk\platform-tools;$env:Path"
```

| Variable | ¿Para qué sirve? |
| --- | --- |
| `JAVA_HOME` | Ubicación del JDK que usarán las herramientas de Android. |
| `ANDROID_HOME` | Carpeta donde se instala el Android SDK. |
| `ANDROID_SDK_ROOT` | Nombre alternativo que algunas herramientas siguen consultando; debe coincidir con `ANDROID_HOME`. |
| `Path` | Facilita ejecutar `java` y `adb` sin escribir sus rutas completas. |

`SetEnvironmentVariable(..., "User")` **guarda** las variables para futuras terminales del usuario. Las asignaciones `$env:...` las aplican **inmediatamente a esta sesión**. El cambio de `Path` mostrado es solo para esta terminal; más adelante usaremos rutas completas para no depender de él.

## 6. Instalar las dependencias del Android SDK

Desde la carpeta de la solución, ejecutar **cambiando `AvaloniaApplication1` por el nombre real** de la app:

```powershell
dotnet build `
  .\AvaloniaApplication1.Android\AvaloniaApplication1.Android.csproj `
  -t:InstallAndroidDependencies `
  -f net10.0-android `
  -p:AndroidSdkDirectory="C:\Android\Sdk" `
  -p:JavaSdkDirectory="$jdk" `
  -p:AcceptAndroidSdkLicenses=True
```

!!! warning "Importante"
    En PowerShell, el símbolo **`** al final de una línea permite continuar el comando en la siguiente. No pongas espacios después de él. Si la aplicación usa otro `TargetFramework`, consulta su `.Android.csproj` y adapta `net10.0-android`.

**Qué hace cada parte:**

- `dotnet build`: invoca la compilación de .NET/MSBuild.
- `.Android.csproj`: identifica el proyecto **Android** de nuestra solución.
- `-t:InstallAndroidDependencies`: instala los componentes de Android necesarios para ese proyecto.
- `-f net10.0-android`: selecciona el destino Android de .NET 10.
- `AndroidSdkDirectory` y `JavaSdkDirectory`: indican explícitamente las carpetas del SDK y el JDK.
- `AcceptAndroidSdkLicenses=True`: acepta las licencias del SDK durante la instalación.

Este paso puede descargar varios componentes y tardar según la conexión. No es necesario repetirlo para cada ejecución; sí puede hacer falta si cambia la versión objetivo de Android.

## 7. Comprobar el Android SDK y localizar sus herramientas

Podemos ver qué carpetas se han instalado (comprobación **opcional**):

```powershell
Get-ChildItem "C:\Android\Sdk" -Directory
```

También se puede buscar `sdkmanager.bat` para comprobar que existen las **Command-line Tools**:

```powershell
Get-ChildItem "C:\Android\Sdk\cmdline-tools" -Recurse -Filter sdkmanager.bat
```

!!! warning "Si no aparece `sdkmanager.bat`"
    Necesitas instalar las **Android SDK Command-line Tools**. Se pueden descargar desde [Android Developers](https://developer.android.com/studio#command-tools) o instalar con **Android Studio → SDK Manager → SDK Tools**. Después, repite la búsqueda. No podremos crear el emulador sin estas herramientas.

Para no buscar dos veces, localizamos `sdkmanager` y obtenemos `avdmanager` desde la **misma carpeta**:

```powershell
$herramienta = Get-ChildItem "C:\Android\Sdk\cmdline-tools" -Recurse -Filter sdkmanager.bat |
    Select-Object -First 1

if (-not $herramienta) { throw "No aparece sdkmanager.bat. Instala Android SDK Command-line Tools." }

$sdkmanager = $herramienta.FullName
$avdmanager = Join-Path (Split-Path $sdkmanager) "avdmanager.bat"
```

- **`sdkmanager`:** descarga y actualiza componentes del Android SDK.
- **`avdmanager`:** crea y administra los dispositivos virtuales Android (**AVD**).

## 8. Descargar los componentes del emulador

```powershell
& $sdkmanager `
    "platform-tools" `
    "emulator" `
    "system-images;android-36;google_apis;x86_64"
```

| Componente | Función |
| --- | --- |
| `platform-tools` | Incluye `adb` para comunicarnos con el emulador o un teléfono físico. |
| `emulator` | Programa que ejecuta dispositivos virtuales Android. |
| `system-images;android-36;google_apis;x86_64` | Imagen de Android **API 36**, con APIs de Google y arquitectura x86-64, para el emulador. |

El operador `&` ejecuta en PowerShell el programa cuya ruta está guardada en una variable. Si se solicitan licencias, aceptarlas; también se pueden revisar con `& $sdkmanager --licenses`.

!!! note "Requisitos del equipo"
    Para el emulador x86-64 necesitamos un ordenador compatible y virtualización por hardware activada. En equipos ARM puede necesitarse una imagen diferente. La descarga de la imagen ocupa espacio en disco.

## 9. Crear el emulador Android

Creamos un dispositivo virtual llamado `CICLO_API36`:

```powershell
"no" | & $avdmanager create avd `
    -n CICLO_API36 `
    -k "system-images;android-36;google_apis;x86_64"
```

- `-n`: nombre del emulador (podemos elegir otro).
- `-k`: imagen de sistema que utilizará.
- `"no" |`: responde **no** a la pregunta opcional de crear un perfil de hardware personalizado; se usa el perfil predeterminado.

Comprobar si se ha creado:

```powershell
& $avdmanager list avd
```

Si ya existe un AVD con el mismo nombre, **no hay que volver a crearlo**.

## 10. Arrancar el emulador y comprobar la conexión

Abrir una terminal y arrancar el emulador:

```powershell
& "C:\Android\Sdk\emulator\emulator.exe" -avd CICLO_API36
```

Dejar esa terminal abierta y esperar a que Android termine de arrancar. Desde **otra terminal**:

```powershell
& "C:\Android\Sdk\platform-tools\adb.exe" devices
```

La salida debería mostrar un identificador similar a `emulator-5554` con estado `device`. Si aparece `offline`, espera a que el emulador arranque completamente y comprueba de nuevo.

## 11. Ejecutar la aplicación Android desde Rider

1. Dejar el emulador encendido.
2. Abrir la solución en **Rider** y verificar que no hay errores de restauración.
3. Seleccionar la configuración correspondiente al proyecto **`AvaloniaApplication1.Android`** y el emulador.
4. Pulsar **Run**.

Si el emulador no aparece en Rider, comprobar primero `adb devices`. Si el proyecto falla por componentes ausentes, repetir `dotnet workload restore` y el apartado 6.

## Resumen: ¿qué pasos se ejecutan una sola vez?

| Paso | ¿Cuándo? |
| --- | --- |
| Instalar .NET, Avalonia Templates, plugin y JDK | Una vez por equipo (o cuando se actualicen). |
| Crear solución y restaurar workloads | Al crear una solución; repetir la restauración si faltan cargas. |
| Variables `JAVA_HOME` y Android | Una vez por usuario, y para la sesión actual. |
| `InstallAndroidDependencies` | Al preparar el SDK o si cambian dependencias/versiones. |
| Instalar imagen y crear AVD | Una vez por emulador. |
| Arrancar emulador y ejecutar desde Rider | Cada vez que queramos probar la aplicación. |

## ¿Y si queremos ejecutar la aplicación en un navegador?

Continúa con [Avalonia en navegador (WebAssembly)](avalonia-en-navegador.md). Es un destino **diferente de Android** y necesita `wasm-tools`, pero no necesita arrancar el emulador.

### Referencias

- [Avalonia: desarrollo para Android](https://docs.avaloniaui.net/docs/platform-specific-guides/android/)
- [Microsoft: instalar dependencias Android](https://learn.microsoft.com/dotnet/android/getting-started/installation/dependencies)
- [Microsoft: configurar Java en Windows](https://learn.microsoft.com/windows/dev-environment/java/)
- [JetBrains: Avalonia en Rider](https://www.jetbrains.com/help/rider/Avalonia.html)
