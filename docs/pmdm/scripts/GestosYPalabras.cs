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
