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
