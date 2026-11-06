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

        string orden = frase.ToLower().Trim().TrimEnd('.', '!', '?');

        // Solo permitimos estas cinco instrucciones.
        if (orden == "avanza cinco casillas" || orden == "avanza 5 casillas")
            orden = "AVANZA";
        else if (orden == "retrocede cinco casillas" || orden == "retrocede 5 casillas")
            orden = "RETROCEDE";
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

        // Añadimos la orden al array.
        Array.Resize(ref instrucciones, instrucciones.Length + 1);
        instrucciones[instrucciones.Length - 1] = orden;

        texto.text = "Órdenes: " + string.Join(" - ", instrucciones);
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

            if (orden == "AVANZA")
                yield return Mover(1);
            else if (orden == "RETROCEDE")
                yield return Mover(-1);
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

    // Mueve el personaje cinco casillas, una a una.
    private IEnumerator Mover(int sentido)
    {
        for (int i = 0; i < 5; i++)
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
