using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class ArrastrarFigura3D : MonoBehaviour
{
    // Opcionales: si el objeto se mueve solo o tiene animaciones.
    public MonoBehaviour movimientoAutomatico;
    public bool usarAnimaciones = false;

    private Camera camara;
    private Rigidbody rb;
    private Animator animator;

    private Plane plano;
    private Vector3 desplazamiento;
    private bool arrastrando;
    private bool tactil;

    void Start()
    {
        // Buscamos la cámara y los componentes del objeto.
        camara = Camera.main;
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        // Si no tenemos cogido el objeto, esperamos un clic o un toque.
        if (!arrastrando)
        {
            if (Touchscreen.current != null &&
                Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
                Coger(Touchscreen.current.primaryTouch.position.ReadValue(), true);
            else if (Mouse.current != null &&
                     Mouse.current.leftButton.wasPressedThisFrame)
                Coger(Mouse.current.position.ReadValue(), false);

            return;
        }

        // Si lo tenemos cogido, seguimos el dedo o el ratón.
        if (tactil)
        {
            if (Touchscreen.current == null ||
                !Touchscreen.current.primaryTouch.press.isPressed)
                Soltar();
            else
                Mover(Touchscreen.current.primaryTouch.position.ReadValue());
        }
        else
        {
            if (Mouse.current == null || !Mouse.current.leftButton.isPressed)
                Soltar();
            else
                Mover(Mouse.current.position.ReadValue());
        }
    }

    void Coger(Vector2 posicion, bool esTactil)
    {
        // Un rayo invisible indica qué Collider hemos pulsado.
        Ray rayo = camara.ScreenPointToRay(posicion);

        // Solo cogemos ESTE objeto o uno de sus hijos.
        if (!Physics.Raycast(rayo, out RaycastHit hit) ||
            !hit.collider.transform.IsChildOf(transform))
            return;

        arrastrando = true;
        tactil = esTactil;

        // Detener cualquier script de movimiento asignado en el Inspector.
        if (movimientoAutomatico != null)
            movimientoAutomatico.enabled = false;

        // Plano horizontal para arrastrar sin cambiar la altura.
        plano = new Plane(Vector3.up, transform.position);
        plano.Raycast(rayo, out float distancia);

        // Guardamos la separación entre el clic y el centro del objeto.
        desplazamiento = transform.position - rayo.GetPoint(distancia);

        // Paramos las físicas mientras sujetamos el objeto.
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        if (usarAnimaciones && animator != null)
        {
            animator.ResetTrigger("Idle");
            animator.SetTrigger("Run");
        }
    }

    void Mover(Vector2 posicion)
    {
        // Calculamos dónde toca el rayo el plano horizontal.
        Ray rayo = camara.ScreenPointToRay(posicion);

        if (plano.Raycast(rayo, out float distancia))
            transform.position = rayo.GetPoint(distancia) + desplazamiento;
    }

    void Soltar()
    {
        arrastrando = false;

        // El objeto vuelve a responder a la gravedad y las colisiones.
        rb.isKinematic = false;

        // Si patrullaba antes, vuelve a moverse automáticamente.
        if (movimientoAutomatico != null)
            movimientoAutomatico.enabled = true;

        if (usarAnimaciones && animator != null)
        {
            animator.ResetTrigger("Run");
            animator.ResetTrigger("Idle");

            // Un objeto con patrulla sigue corriendo; el resto queda en reposo.
            animator.SetTrigger(movimientoAutomatico != null ? "Run" : "Idle");
        }
    }
}
