using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class ArrastrarFigura3D : MonoBehaviour
{
    private Camera camara;
    private Rigidbody rb;
    private Animator animator;

    private Plane plano;
    private Vector3 desplazamiento;
    private bool arrastrando;
    private bool tactil;

    // Actívalo en el Inspector solo si el Animator tiene los triggers Run e Idle.
    public bool usarAnimaciones = false;

    void Start()
    {
        camara = Camera.main;
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        // Comenzar a arrastrar con el dedo o el botón izquierdo del ratón.
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

        // Mientras mantenemos pulsado, mover el objeto; al soltar, recuperar la física.
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
        // El Raycast identifica el objeto pulsado (también si su Collider está en un hijo).
        Ray rayo = camara.ScreenPointToRay(posicion);
        if (!Physics.Raycast(rayo, out RaycastHit hit) ||
            !hit.collider.transform.IsChildOf(transform))
            return;

        arrastrando = true;
        tactil = esTactil;

        // Plano horizontal invisible: desplazamos la figura sin cambiar su altura.
        plano = new Plane(Vector3.up, transform.position);
        plano.Raycast(rayo, out float distancia);
        desplazamiento = transform.position - rayo.GetPoint(distancia);

        // Suspender temporalmente la gravedad y las fuerzas durante el arrastre.
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
        Ray rayo = camara.ScreenPointToRay(posicion);

        if (plano.Raycast(rayo, out float distancia))
            transform.position = rayo.GetPoint(distancia) + desplazamiento;
    }

    void Soltar()
    {
        arrastrando = false;

        // Volver a activar las físicas: si está sobre un agujero real, caerá.
        rb.isKinematic = false;

        if (usarAnimaciones && animator != null)
        {
            animator.ResetTrigger("Run");
            animator.SetTrigger("Idle");
        }
    }
}
