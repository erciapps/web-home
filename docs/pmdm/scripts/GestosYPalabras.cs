using System;
using System.Reflection;
using TMPro;
using UnityEngine;
using HandForge.HandTracking;
using Mediapipe.Tasks.Vision.GestureRecognizer;

// Preparación para el siguiente minijuego: solo muestra gestos y palabras.
public class GestosYPalabras : MonoBehaviour, ISpeechToTextListener
{
    public HandTrackingManager tracker;
    public TMP_Text textoGestos;
    public TMP_Text textoVoz;

    private FieldInfo campoResultado;

    void Start()
    {
        SpeechToText.Initialize("es-ES");

        // Leemos el resultado existente sin modificar archivos del paquete.
        campoResultado = typeof(MediaPipeHandProvider).GetField(
            "_result", BindingFlags.NonPublic | BindingFlags.Instance);
    }

    void Update()
    {
        if (tracker == null || !tracker.IsReady)
        {
            textoGestos.text = "Detector no iniciado";
            return;
        }

        if (!tracker.RightHand.IsTracked && !tracker.LeftHand.IsTracked)
        {
            textoGestos.text = "No se detecta ninguna mano";
            return;
        }

        var proveedor = tracker.Subsystem.Provider as MediaPipeHandProvider;
        if (proveedor == null || campoResultado == null)
        {
            textoGestos.text = "No disponible";
            return;
        }

        var resultado = (GestureRecognizerResult)campoResultado.GetValue(proveedor);
        string gesto = "GESTO NO RECONOCIDO";
        float confianza = 0.7f;

        if (resultado.gestures != null)
        {
            foreach (var mano in resultado.gestures)
            {
                if (mano.categories == null)
                    continue;

                foreach (var categoria in mano.categories)
                {
                    if (categoria.score < confianza)
                        continue;

                    switch (categoria.categoryName)
                    {
                        case "Open_Palm":
                            gesto = "MANO ABIERTA";
                            break;
                        case "Closed_Fist":
                            gesto = "PUÑO CERRADO";
                            break;
                        case "Thumb_Up":
                            gesto = "PULGAR ARRIBA";
                            break;
                        case "Thumb_Down":
                            gesto = "PULGAR ABAJO";
                            break;
                        default:
                            continue;
                    }

                    confianza = categoria.score;
                }
            }
        }

        textoGestos.text = gesto;
    }

    // Conectar este método al botón de micrófono.
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
            if (permiso != SpeechToText.Permission.Granted)
            {
                textoVoz.text = "Permiso de micrófono denegado";
                return;
            }

            if (!SpeechToText.Start(this))
                textoVoz.text = "No se pudo escuchar";
        });
    }

    public void OnReadyForSpeech()
    {
        textoVoz.text = "Escuchando...";
    }

    public void OnBeginningOfSpeech() { }
    public void OnVoiceLevelChanged(float nivel) { }

    public void OnPartialResultReceived(string palabras)
    {
        textoVoz.text = "Escuchando: " + palabras;
    }

    public void OnResultReceived(string palabras, int? error)
    {
        if (error.HasValue || string.IsNullOrWhiteSpace(palabras))
        {
            textoVoz.text = "Palabra no reconocida";
            return;
        }

        textoVoz.text = "Has dicho: " + palabras;
    }
}
