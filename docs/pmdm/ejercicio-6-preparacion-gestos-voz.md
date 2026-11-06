# Ejercicio 6 · Preparación de gestos y palabras

Prepararemos las **dependencias y un script** para el siguiente minijuego de Unity 3D en Android. Por ahora **no construiremos escenarios ni mecánicas**: solo comprobaremos que podemos leer voz y gestos en pantalla.

## 1. Dependencias

Abre **Window → Package Manager → + → Install package from git URL**.

**Reconocimiento de voz** (si ya lo instalaste en el ejercicio 5, no lo repitas):

~~~text
https://github.com/yasirkula/UnitySpeechToText.git
~~~

**HandForge · runtime de MediaPipe:**

~~~text
https://github.com/ShonubiSamuel/HandForge.git?path=/Packages/com.samuelshonubi.handtracking
~~~

**HandForge · componentes de Unity:**

~~~text
https://github.com/ShonubiSamuel/HandForge.git?path=/Packages/com.handforge.handtracking
~~~

!!! warning "Android"
    HandForge incluye bibliotecas para Android, pero el paquete que hemos utilizado no trae **mediapipe_c.dll** para ejecutar su backend en **Unity Editor sobre Windows**. Hay que probar el reconocimiento de manos con **Build And Run** en un dispositivo Android.

## 2. Configuración mínima de HandForge

1. Añade un objeto **HandManager** con componente **Hand Tracking Manager**.
2. Configura **Source Kind: Webcam**, **Camera Facing: Front**, **Max Hand Count: 1** y **Origin: Main Camera**.
3. Abre **Edit → Project Settings → Hand Tracking** y verifica que el modelo **gesture_recognizer.bytes** está asignado. Si no aparece, utiliza **Locate Model**.
4. En el Canvas crea dos textos TextMeshPro: **TextoGestos** y **TextoVoz**.
5. Añade un botón **ESCUCHAR**. No necesitamos mostrar la cámara: el Raw Image es opcional.

### Conservar el backend en Android

Crea un archivo **Assets/link.xml** (archivo de texto, no link.xml.txt) con el contenido:

~~~xml
<linker>
    <assembly fullname="HandForge.MediaPipe" preserve="all" />
</linker>
~~~

Esto evita que Unity elimine el ensamblado que registra el backend MediaPipe durante el proceso de compilación. También conserva el campo privado que utiliza el script.

## 3. Script GestosYPalabras.cs

Descarga o copia **[GestosYPalabras.cs](scripts/GestosYPalabras.cs)** en **Assets/Scripts**.

El script:

- Muestra **MANO ABIERTA**, **PUÑO CERRADO**, **PULGAR ARRIBA** y **PULGAR ABAJO**, sin distinguir izquierda/derecha.
- Lee la clasificación de gestos del modelo MediaPipe, en lugar de aproximarla con la flexión de los dedos.
- Muestra el texto de voz reconocido al pulsar el botón.
- Todavía **no hace que el personaje se mueva ni da puntos**; eso será parte del siguiente minijuego.

!!! note "Sin modificar paquetes"
    HandForge no expone públicamente la clasificación de gestos de MediaPipe. Para la práctica leemos su campo interno mediante **reflexión**, desde nuestro script. No se edita ningún archivo del plugin; este acceso es un atajo que podría requerir adaptación si cambia la versión de HandForge.

## 4. Conectar y comprobar

1. Crea un objeto **Controlador** y añádele **GestosYPalabras.cs**.
2. Arrastra **HandManager**, **TextoGestos** y **TextoVoz** a sus campos.
3. En el botón **ESCUCHAR → On Click()**, conecta **Controlador → GestosYPalabras → Escuchar()**.
4. Ejecuta **Build And Run** en Android y acepta permisos de cámara y micrófono.
5. Prueba los cuatro gestos, y pronuncia una palabra para comprobar que aparece en TextoVoz.

Por ahora no necesitamos más escenas, objetos ni scripts.
