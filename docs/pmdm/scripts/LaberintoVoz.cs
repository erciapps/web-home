using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
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

    // Se puede consultar el array en el Inspector durante Play.
    [SerializeField] private string[] instrucciones = new string[0];

    private bool ejecutando;
    private bool terminado;
    private bool bloqueado;

    void Start()
    {
        SpeechToText.Initialize("es-ES");
        texto.text = "Pulsa ESCUCHAR y di una instrucción";
    }

    // Conectar este método al botón ESCUCHAR.
    public void Escuchar()
    {
        if (ejecutando || terminado || SpeechToText.IsBusy())
            return;

        if (!SpeechToText.IsServiceAvailable())
        {
            texto.text = "Reconocimiento de voz no disponible";
            return;
        }

        SpeechToText.RequestPermissionAsync(permiso =>
        {
            if (permiso != SpeechToText.Permission.Granted)
            {
                texto.text = "Permiso de micrófono denegado";
                return;
            }

            if (!SpeechToText.Start(this))
                texto.text = "No se pudo iniciar el micrófono";
        });
    }

    public void OnReadyForSpeech()
    {
        texto.text = "Escuchando...";
    }

    public void OnBeginningOfSpeech() { }
    public void OnVoiceLevelChanged(float nivel) { }

    // Mostramos el texto provisional, pero NO ejecutamos órdenes todavía.
    public void OnPartialResultReceived(string frase)
    {
        texto.text = "Escuchando: " + frase;
    }

    // Solo procesamos el resultado final, una vez por pulsación.
    public void OnResultReceived(string frase, int? error)
    {
        if (error.HasValue || string.IsNullOrWhiteSpace(frase))
        {
            texto.text = "No se reconoció la instrucción";
            return;
        }

        if (ejecutando || terminado)
            return;

        if (!ConvertirEnArray(frase, out instrucciones))
        {
            texto.text = "Orden no válida: " + frase;
            return;
        }

        texto.text = "Órdenes: " + string.Join(" | ", instrucciones);
        StartCoroutine(Ejecutar());
    }

    // Aceptamos únicamente las instrucciones de la actividad.
    private bool ConvertirEnArray(string frase, out string[] resultado)
    {
        List<string> lista = new List<string>();
        string[] palabras = Normalizar(frase).Split(
            new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        int i = 0;

        while (i < palabras.Length)
        {
            // Permite encadenar: "avanza cinco casillas y gira derecha".
            if (palabras[i] == "y")
            {
                i++;
                continue;
            }

            if (palabras[i] == "avanza" || palabras[i] == "retrocede")
            {
                if (i + 2 >= palabras.Length ||
                    (palabras[i + 1] != "5" && palabras[i + 1] != "cinco") ||
                    (palabras[i + 2] != "casilla" && palabras[i + 2] != "casillas"))
                {
                    resultado = new string[0];
                    return false;
                }

                lista.Add(palabras[i] == "avanza" ? "AVANZA" : "RETROCEDE");
                i += 3;
                continue;
            }

            if (palabras[i] == "gira")
            {
                i++;
                // Acepta "gira derecha" y "gira a la derecha".
                if (i < palabras.Length && palabras[i] == "a")
                    i++;
                if (i < palabras.Length && palabras[i] == "la")
                    i++;

                if (i >= palabras.Length ||
                    (palabras[i] != "izquierda" && palabras[i] != "derecha"))
                {
                    resultado = new string[0];
                    return false;
                }

                lista.Add(palabras[i] == "izquierda" ? "IZQUIERDA" : "DERECHA");
                i++;
                continue;
            }

            if (i + 2 < palabras.Length &&
                palabras[i] == "da" &&
                palabras[i + 1] == "la" &&
                palabras[i + 2] == "vuelta")
            {
                lista.Add("VUELTA");
                i += 3;
                continue;
            }

            // Una palabra extraña invalida la frase completa.
            resultado = new string[0];
            return false;
        }

        resultado = lista.ToArray();
        return resultado.Length > 0;
    }

    private string Normalizar(string frase)
    {
        string sinTildes = frase.ToLowerInvariant().Normalize(NormalizationForm.FormD);
        StringBuilder limpio = new StringBuilder();

        foreach (char letra in sinTildes)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(letra) == UnicodeCategory.NonSpacingMark)
                continue;

            limpio.Append(char.IsLetterOrDigit(letra) ? letra : ' ');
        }

        return limpio.ToString();
    }

    private IEnumerator Ejecutar()
    {
        ejecutando = true;
        bloqueado = false;

        foreach (string instruccion in instrucciones)
        {
            switch (instruccion)
            {
                case "AVANZA":
                    yield return Mover(1);
                    break;

                case "RETROCEDE":
                    yield return Mover(-1);
                    break;

                case "IZQUIERDA":
                    jugador.Rotate(0f, -90f, 0f, Space.World);
                    break;

                case "DERECHA":
                    jugador.Rotate(0f, 90f, 0f, Space.World);
                    break;

                case "VUELTA":
                    jugador.Rotate(0f, 180f, 0f, Space.World);
                    break;
            }

            if (terminado || bloqueado)
                break;
        }

        if (terminado)
            texto.text = "¡META CONSEGUIDA!";
        else if (bloqueado)
            texto.text = "¡Obstáculo o borde! Inténtalo de nuevo";
        else
            texto.text = "Ejecutado: " + string.Join(" | ", instrucciones);

        ejecutando = false;
    }

    // Avanza o retrocede siempre 5 casillas, una detrás de otra.
    private IEnumerator Mover(int sentido)
    {
        for (int paso = 0; paso < 5; paso++)
        {
            Vector3 direccion = jugador.forward * sentido;
            Vector3 desplazamiento = new Vector3(
                Mathf.RoundToInt(direccion.x), 0f,
                Mathf.RoundToInt(direccion.z));

            Vector3 destino = jugador.position + desplazamiento;

            // Tablero: casillas con coordenadas X y Z de 0 a 7.
            if (destino.x < 0 || destino.x >= columnas ||
                destino.z < 0 || destino.z >= filas ||
                Physics.CheckBox(
                    destino + Vector3.up * 0.5f,
                    new Vector3(0.35f, 0.45f, 0.35f),
                    Quaternion.identity,
                    obstaculos,
                    QueryTriggerInteraction.Ignore))
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

            // Comprobamos la meta después de cada casilla.
            if (meta != null &&
                Mathf.Abs(jugador.position.x - meta.position.x) < 0.1f &&
                Mathf.Abs(jugador.position.z - meta.position.z) < 0.1f)
            {
                terminado = true;
                yield break;
            }
        }
    }
}
