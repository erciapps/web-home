# Ejercicio 5 · Laberinto 3D controlado por voz

Crearemos un minijuego en **Unity 6 para Android**: un personaje 3D de KayKit tiene que llegar a la meta de un laberinto. Lo controlaremos **únicamente con la voz**.

## 1. Recursos e instalación

Crea un proyecto **Universal 3D (URP)**. Descarga de itch.io:

- [KayKit Adventurers](https://kaylousberg.itch.io/kaykit-adventurers) — personaje 3D.
- [KayKit Prototype Bits](https://kaylousberg.itch.io/prototype-bits) — obstáculos estáticos.

Para instalar el reconocimiento de voz, abre **Window → Package Manager → + → Install package from git URL** y pega:

~~~text
https://github.com/yasirkula/UnitySpeechToText.git
~~~

El paquete convierte la voz en texto. En Android pediremos permiso de micrófono cuando pulsemos **ESCUCHAR**.

## 2. Crear el tablero

1. Guarda la escena como **LaberintoVoz**.
2. Construye un tablero de **8 × 8 casillas** con cubos de escala **(1, 0.2, 1)**. Los centros deben estar en coordenadas enteras, X = 0 a 7 y Z = 0 a 7. Coloca los cubos para que su superficie superior esté en Y = 0.
3. Coloca obstáculos de KayKit sobre algunas casillas. Añade a cada uno un **Box Collider**, crea la capa **Obstaculos** y asígnasela. Los obstáculos son estáticos, sin Rigidbody.
4. Añade un objeto **Meta** en una casilla libre.
5. Crea un GameObject vacío **Jugador** en otra casilla libre, con X y Z enteros e Y = 0. Mete el modelo de KayKit como hijo y ajusta su altura. El eje Z positivo de Jugador será su frente.
6. Coloca una cámara elevada que permita ver el laberinto completo.

!!! important
    El suelo y la meta **no** deben estar en la capa Obstaculos. No añadiremos Rigidbody ni Character Controller al jugador: nos moveremos por casillas mediante el script.

## 3. Instrucciones por voz

| Lo que decimos | Lo que hace el personaje |
| --- | --- |
| **Avanza 5** / **Avanza tres casillas** | Avanza 5 o 3 casillas, respectivamente |
| **Retrocede 2** / **Retrocede una casilla** | Retrocede 2 o 1 casillas, respectivamente |
| **Gira izquierda** | Gira 90° a la izquierda |
| **Gira derecha** | Gira 90° a la derecha |
| **Da la vuelta** | Gira 180° en sentido horario |

La cantidad de casillas **ya no está fija en cinco**. Se admite cualquier entero positivo escrito con cifras (por ejemplo, «avanza 12»). Si el reconocimiento devuelve palabras, el script también interpreta los números **del uno al veinte**. Se puede decir «casilla» o «casillas», o no indicar esa palabra.

También se aceptan «gira a la izquierda» y «gira a la derecha».

**Funcionamiento:** pulsa **ESCUCHAR**, di **una instrucción** y añádela al array. Repite tantas veces como quieras. Al pulsar **EJECUTAR**, el personaje ejecuta las instrucciones en orden, deteniéndose si encuentra un obstáculo, sale del tablero o llega a la meta.

Por ejemplo, al escuchar cinco instrucciones, el array contendrá:

~~~csharp
["AVANZA 5", "DERECHA", "AVANZA 3", "IZQUIERDA", "RETROCEDE 2"]
~~~

!!! tip "Si aparece «Orden no válida»"
    Abre la consola de Unity y comprueba el mensaje **Voz reconocida: [...]**. Puede que el servicio devuelva «cinco» en vez de «5», añada palabras extra o no reconozca bien la instrucción. En el Editor de Unity la voz se simula; la prueba real se hace en Android.

## 4. Script LaberintoVoz.cs

Crea el archivo **Assets/Scripts/LaberintoVoz.cs** y copia este código actualizado. Ya incluye reconocimiento de números y movimiento variable:

~~~csharp
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class LaberintoVoz : MonoBehaviour, ISpeechToTextListener
{
    public Transform jugador;
    public Transform meta;
    public TMP_Text texto;
    public LayerMask obstaculos;
    public int columnas = 8;
    public int filas = 8;
    public float velocidad = 3f;

    // Guardamos las instrucciones reconocidas en un array.
    public string[] instrucciones = new string[0];

    private bool ejecutando;
    private bool bloqueado;
    private bool terminado;

    void Start()
    {
        SpeechToText.Initialize("es-ES");
        texto.text = "Pulsa ESCUCHAR";
    }

    // Se llama desde el botón ESCUCHAR.
    public void Escuchar()
    {
        if (ejecutando || terminado || SpeechToText.IsBusy())
            return;

        if (!SpeechToText.IsServiceAvailable())
        {
            texto.text = "Voz no disponible";
            return;
        }

        SpeechToText.RequestPermissionAsync(permiso =>
        {
            if (permiso == SpeechToText.Permission.Granted)
            {
                if (!SpeechToText.Start(this))
                    texto.text = "No se pudo iniciar el micrófono";
            }
            else
                texto.text = "Permiso de micrófono denegado";
        });
    }

    // Recibimos una instrucción completa cada vez.
    public void OnResultReceived(string frase, int? error)
    {
        if (error.HasValue || string.IsNullOrWhiteSpace(frase))
        {
            texto.text = "No se reconoció la voz";
            return;
        }

        // Permite comprobar qué ha entendido realmente el reconocedor.
        Debug.Log("Voz reconocida: [" + frase + "]");

        string orden = frase.ToLowerInvariant().Trim().TrimEnd('.', '!', '?', ',', ' ');
        string[] partes = orden.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        // Reconocemos números variables: "avanza 5", "retrocede dos",
        // "avanza tres casillas" o "retrocede 1 casilla".
        if (partes.Length >= 2 &&
            (partes[0] == "avanza" || partes[0] == "retrocede"))
        {
            int casillas = ObtenerNumero(partes[1]);
            bool formatoCorrecto = partes.Length == 2 ||
                (partes.Length == 3 &&
                 (partes[2] == "casilla" || partes[2] == "casillas"));

            if (casillas > 0 && formatoCorrecto)
                orden = partes[0].ToUpperInvariant() + " " + casillas;
            else
            {
                texto.text = "Orden no válida: " + frase;
                return;
            }
        }
        else if (orden == "gira izquierda" || orden == "gira a la izquierda")
            orden = "IZQUIERDA";
        else if (orden == "gira derecha" || orden == "gira a la derecha")
            orden = "DERECHA";
        else if (orden == "da la vuelta")
            orden = "VUELTA";
        else
        {
            texto.text = "Orden no válida: " + frase;
            return;
        }

        // Añadimos la orden y su cantidad al array.
        Array.Resize(ref instrucciones, instrucciones.Length + 1);
        instrucciones[instrucciones.Length - 1] = orden;

        texto.text = "Órdenes: " + string.Join(" - ", instrucciones);
    }

    // El reconocedor puede devolver cifras ("5") o palabras ("cinco").
    private int ObtenerNumero(string palabra)
    {
        if (int.TryParse(palabra, out int numero))
            return numero;

        switch (palabra)
        {
            case "uno":
            case "un":
            case "una": return 1;
            case "dos": return 2;
            case "tres": return 3;
            case "cuatro": return 4;
            case "cinco": return 5;
            case "seis": return 6;
            case "siete": return 7;
            case "ocho": return 8;
            case "nueve": return 9;
            case "diez": return 10;
            case "once": return 11;
            case "doce": return 12;
            case "trece": return 13;
            case "catorce": return 14;
            case "quince": return 15;
            case "dieciséis":
            case "dieciseis": return 16;
            case "diecisiete": return 17;
            case "dieciocho": return 18;
            case "diecinueve": return 19;
            case "veinte": return 20;
            default: return -1;
        }
    }

    // Se llama desde el botón EJECUTAR.
    public void Ejecutar()
    {
        if (ejecutando || terminado || SpeechToText.IsBusy() || instrucciones.Length == 0)
            return;

        StartCoroutine(Recorrer());
    }

    private IEnumerator Recorrer()
    {
        ejecutando = true;
        bloqueado = false;

        foreach (string orden in instrucciones)
        {
            texto.text = "Ejecutando: " + orden;

            string[] partes = orden.Split(' ');

            if (partes[0] == "AVANZA")
                yield return Mover(1, int.Parse(partes[1]));
            else if (partes[0] == "RETROCEDE")
                yield return Mover(-1, int.Parse(partes[1]));
            else if (orden == "IZQUIERDA")
                jugador.Rotate(0, -90, 0);
            else if (orden == "DERECHA")
                jugador.Rotate(0, 90, 0);
            else if (orden == "VUELTA")
                jugador.Rotate(0, 180, 0);

            if (bloqueado || terminado)
                break;
        }

        instrucciones = new string[0];

        if (terminado)
            texto.text = "¡META CONSEGUIDA!";
        else if (bloqueado)
            texto.text = "Obstáculo o borde del tablero";
        else
            texto.text = "Recorrido terminado";

        ejecutando = false;
    }

    // Mueve el personaje tantas casillas como indique la orden.
    private IEnumerator Mover(int sentido, int casillas)
    {
        for (int i = 0; i < casillas; i++)
        {
            Vector3 direccion = jugador.forward * sentido;

            Vector3 destino = new Vector3(
                Mathf.RoundToInt(jugador.position.x + direccion.x),
                jugador.position.y,
                Mathf.RoundToInt(jugador.position.z + direccion.z));

            bool fuera = destino.x < 0 || destino.x >= columnas ||
                         destino.z < 0 || destino.z >= filas;

            bool hayObstaculo = Physics.CheckBox(
                destino + Vector3.up * 0.5f,
                new Vector3(0.4f, 0.45f, 0.4f),
                Quaternion.identity, obstaculos);

            if (fuera || hayObstaculo)
            {
                bloqueado = true;
                yield break;
            }

            while (Vector3.Distance(jugador.position, destino) > 0.01f)
            {
                jugador.position = Vector3.MoveTowards(
                    jugador.position, destino, velocidad * Time.deltaTime);
                yield return null;
            }

            jugador.position = destino;

            if (meta != null &&
                Mathf.Abs(jugador.position.x - meta.position.x) < 0.1f &&
                Mathf.Abs(jugador.position.z - meta.position.z) < 0.1f)
            {
                terminado = true;
                yield break;
            }
        }
    }

    // Métodos obligatorios del plugin de voz.
    public void OnReadyForSpeech() { texto.text = "Escuchando..."; }
    public void OnBeginningOfSpeech() { }
    public void OnVoiceLevelChanged(float nivel) { }
    public void OnPartialResultReceived(string frase) { }
}
~~~

### Qué hace cada parte

- **Start:** inicializa el reconocimiento de voz en español.
- **Escuchar:** solicita permiso y activa el micrófono.
- **OnResultReceived:** normaliza la frase, comprueba si es válida y añade la orden al array.
- **ObtenerNumero:** interpreta cifras y números escritos con palabras del uno al veinte.
- **Ejecutar / Recorrer:** lee cada orden; en movimientos separa el verbo y la cantidad.
- **Mover:** avanza o retrocede el número indicado de casillas sin atravesar obstáculos, abandonar el tablero ni saltarse la meta.

## 5. Conectar el script en Unity

En el Canvas crea un **Text - TextMeshPro** y dos botones: **ESCUCHAR** y **EJECUTAR**.

Crea un GameObject vacío llamado **Controlador**, añade **LaberintoVoz.cs** y configura sus campos:

| Campo del Inspector | Valor |
| --- | --- |
| Jugador | Objeto Jugador |
| Meta | Objeto Meta |
| Texto | TextMeshPro del Canvas |
| Obstaculos | Solo la capa Obstaculos |
| Columnas | 8 |
| Filas | 8 |
| Velocidad | 3 |

En el evento **On Click()** de cada botón, arrastra Controlador:

- Botón **ESCUCHAR** → función **LaberintoVoz.Escuchar()**.
- Botón **EJECUTAR** → función **LaberintoVoz.Ejecutar()**.

El array **Instrucciones** se puede consultar en el Inspector mientras el juego está en Play.

## 6. Probar en Android

Abre **File → Build Profiles → Android → Build And Run** y concede acceso al micrófono.

Prueba **«avanza 5»**, **«avanza tres casillas»**, **«retrocede 2»** y los giros. Comprueba que el array almacena la cantidad, los movimientos respetan esa cantidad, los obstáculos bloquean el paso y aparece **¡META CONSEGUIDA!** al llegar a la meta.

!!! note
    En Unity Editor, Speech to Text simula respuestas; la prueba real de voz debe hacerse en el móvil. El servicio de reconocimiento de Android debe estar disponible.

**[Ver también el archivo LaberintoVoz.cs](scripts/LaberintoVoz.cs)**
