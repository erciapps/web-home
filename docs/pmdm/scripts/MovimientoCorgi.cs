using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovimientoCorgi : MonoBehaviour
{
    public Transform puntoA;
    public Transform puntoB;
    public float velocidad = 2f;

    private Rigidbody rb;
    private Animator animator;
    private Transform destino;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();

        if (puntoA == null || puntoB == null) return;

        // Comenzar en A y dirigirse hacia B.
        rb.position = puntoA.position;
        destino = puntoB;

        if (animator != null)
            animator.SetTrigger("Run");
    }

    void FixedUpdate()
    {
        if (destino == null) return;

        // Dirección horizontal: no modificamos la velocidad vertical de la gravedad.
        Vector3 direccion = destino.position - rb.position;
        direccion.y = 0;

        // Al llegar a un extremo, cambiar de destino: A -> B -> A -> B...
        if (direccion.magnitude < 0.1f)
        {
            destino = destino == puntoB ? puntoA : puntoB;
            return;
        }

        direccion.Normalize();

        // Avanzar manteniendo la velocidad vertical del Rigidbody.
        rb.linearVelocity = new Vector3(
            direccion.x * velocidad,
            rb.linearVelocity.y,
            direccion.z * velocidad
        );

        // Mirar hacia la dirección del desplazamiento.
        rb.MoveRotation(Quaternion.LookRotation(direccion));
    }
}
