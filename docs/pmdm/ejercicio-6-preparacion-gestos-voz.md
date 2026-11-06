# Ejercicio 6 · Preparación de gestos y palabras

Preparamos **dependencias y un único script** para el siguiente minijuego. Por ahora solo mostraremos el gesto y las palabras que reconoce el móvil: **sin escenarios ni mecánicas**.

## 1. Instalar dependencias

Abre **Window → Package Manager → + → Install package from git URL**.

**Speech to Text** (si lo instalaste en el ejercicio 5, no hace falta repetirlo):

~~~text
https://github.com/yasirkula/UnitySpeechToText.git
~~~

**HandForge · motor MediaPipe:**

~~~text
https://github.com/ShonubiSamuel/HandForge.git?path=/Packages/com.samuelshonubi.handtracking
~~~

**HandForge · componentes Unity:**

~~~text
https://github.com/ShonubiSamuel/HandForge.git?path=/Packages/com.handforge.handtracking
~~~

## 2. Configurar Unity

1. Crea **HandManager** y añade el componente **Hand Tracking Manager**.
2. Selecciona **Source Kind: Webcam**, **Camera Facing: Front**, **Max Hand Count: 1** y **Origin: Main Camera**.
3. Abre **Edit → Project Settings → Hand Tracking**. Comprueba que el modelo **gesture_recognizer.bytes** está asignado. Si falta, pulsa **Locate Model**.
4. En el Canvas crea dos TextMeshPro: **TextoGestos** y **TextoVoz**, y un botón **ESCUCHAR**.

Crea el archivo **Assets/link.xml** desde el Explorador de Windows, asegurándote de que no termina en .txt:

~~~xml
<linker>
    <assembly fullname="HandForge.MediaPipe" preserve="all" />
</linker>
~~~

Esto conserva el backend MediaPipe en la compilación Android. No hay que modificar los archivos del plugin.

## 3. Script GestosYPalabras.cs

Crea **Assets/Scripts/GestosYPalabras.cs** y copia el código completo:

~~~csharp
using System.Reflection;
using TMPro;
using UnityEngine;
using HandForge.HandTracking;
using Mediapipe.Tasks.Vision.GestureRecognizer;

public class GestosYPalabras : MonoBehaviour, ISpeechToTextListener
{
    public HandTrackingManager tracker;
    public TMP_Text textoGestos;
    public TMP_Text textoVoz;

    private FieldInfo campo;

    void Start()
    {
        SpeechToText.Initialize("es-ES");

        // Leemos el resultado del detector sin modificar el plugin.
        campo = typeof(MediaPipeHandProvider).GetField(
            "_result", BindingFlags.NonPublic | BindingFlags.Instance);
    }

    void Update()
    {
        if (!tracker.IsReady)
        {
            textoGestos.text = "Detector no iniciado";
            return;
        }

        if (!tracker.RightHand.IsTracked && !tracker.LeftHand.IsTracked)
        {
            textoGestos.text = "No se detecta ninguna mano";
            return;
        }

        var detector = tracker.Subsystem.Provider as MediaPipeHandProvider;
        if (detector == null || campo == null)
            return;

        var resultado = (GestureRecognizerResult)campo.GetValue(detector);
        textoGestos.text = "Gesto no reconocido";

        if (resultado.gestures == null)
            return;

        foreach (var mano in resultado.gestures)
        {
            if (mano.categories == null)
                continue;

            foreach (var categoria in mano.categories)
            {
                if (categoria.score < 0.7f)
                    continue;

                if (categoria.categoryName == "Open_Palm")
                    textoGestos.text = "MANO ABIERTA";
                else if (categoria.categoryName == "Closed_Fist")
                    textoGestos.text = "PUÑO CERRADO";
                else if (categoria.categoryName == "Thumb_Up")
                    textoGestos.text = "PULGAR ARRIBA";
                else if (categoria.categoryName == "Thumb_Down")
                    textoGestos.text = "PULGAR ABAJO";
            }
        }
    }

    // Botón ESCUCHAR.
    public void Escuchar()
    {
        if (SpeechToText.IsBusy())
            return;

        if (!SpeechToText.IsServiceAvailable())
        {
            textoVoz.text = "Voz no disponible";
            return;
        }

        SpeechToText.RequestPermissionAsync(permiso =>
        {
            if (permiso == SpeechToText.Permission.Granted)
            {
                if (!SpeechToText.Start(this))
                    textoVoz.text = "No se pudo escuchar";
            }
            else
                textoVoz.text = "Permiso de micrófono denegado";
        });
    }

    public void OnReadyForSpeech() { textoVoz.text = "Escuchando..."; }
    public void OnBeginningOfSpeech() { }
    public void OnVoiceLevelChanged(float nivel) { }
    public void OnPartialResultReceived(string frase) { }

    public void OnResultReceived(string frase, int? error)
    {
        if (error.HasValue || string.IsNullOrWhiteSpace(frase))
            textoVoz.text = "No se reconoció la voz";
        else
            textoVoz.text = "Has dicho: " + frase;
    }
}
~~~

Este script muestra cuatro gestos (**mano abierta, puño cerrado, pulgar arriba y pulgar abajo**) sin distinguir izquierda o derecha. El botón de voz muestra la frase reconocida.

!!! note "Una particularidad de HandForge"
    El paquete guarda la clasificación de gestos en un campo privado. Por eso utilizamos **FieldInfo** para leerlo sin editar el plugin. Es un atajo válido para probar esta versión, pero depende de sus detalles internos.

## 4. Conectar y comprobar

1. Crea un objeto **Controlador** y añade el script **GestosYPalabras.cs**.
2. Arrastra **HandManager**, **TextoGestos** y **TextoVoz** a sus campos.
3. En el botón **ESCUCHAR → On Click()**, selecciona **Controlador → GestosYPalabras.Escuchar()**.
4. Pulsa **Build And Run** para Android y concede los permisos de cámara y micrófono.

!!! warning
    El paquete instalado no incluye la DLL nativa de MediaPipe para ejecutar el detector en el Editor de Windows; prueba los gestos en Android.

**[Ver también el archivo GestosYPalabras.cs](scripts/GestosYPalabras.cs)**
