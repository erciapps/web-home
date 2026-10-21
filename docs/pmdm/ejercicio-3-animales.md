# Ejercicio 3 · Clasificador de animales 3D

Vamos a preparar un juego para clasificar animales en tres cestas: **mamíferos, reptiles y aves**. Los animales podrán moverse por la plataforma y podremos cogerlos con el ratón o con el dedo para llevarlos a una cesta.

!!! info "Qué vamos a hacer ahora"
    Empezaremos con los modelos, las cestas, el movimiento automático y el arrastre. **Todavía no vamos a comprobar si elegimos la cesta correcta**; esa parte la añadiremos después.

## 1. Descargar y colocar los animales

Descarga los modelos de **[Cube Pets Vol. 3 (itch.io)](https://sneznyj.itch.io/cube-pets-vol-3?download)** e impórtalos en Unity.

Crea una escena `Animals` y prepara:

1. Una plataforma con Collider.
2. Tres cestas o zonas llamadas `CestaMamiferos`, `CestaReptiles` y `CestaAves`.
3. Los animales que vayas a utilizar, cada uno con su **Rigidbody** y su **Collider**.
4. Dos objetos vacíos llamados `PuntoA` y `PuntoB` para probar el desplazamiento.

!!! warning "Comprobar el Box Collider"
    El modelo 3D se ve en pantalla gracias a su malla, pero para seleccionarlo con un clic necesitamos que el rayo toque un **Collider**. Añade un `Box Collider` al animal y ajusta su tamaño con **Edit Collider**, sin incluir demasiado espacio vacío. Si el perro corre pero no podemos cogerlo, lo primero que debemos revisar es esto. Si usamos Mesh Collider en un objeto con Rigidbody dinámico, debe ser **Convex**.

## 2. Script · MovimientoAnimales.cs

Queremos que un animal se mueva de `PuntoA` a `PuntoB` y, cuando llegue, regrese a `PuntoA`. Después repetirá el recorrido continuamente.

El nombre es **MovimientoAnimales** porque el script sirve para un perro, un gato o cualquier otro animal.

**[Abrir o descargar MovimientoAnimales.cs desde GitHub](https://github.com/erciapps/web-home/blob/main/docs/pmdm/scripts/MovimientoAnimales.cs)**

### Paso 1. Indicar los puntos y la velocidad

```csharp
public Transform puntoA;
public Transform puntoB;
public float velocidad = 2f;
```

`Transform` nos permite conocer la posición de cada punto. Como las variables son públicas, aparecerán en el **Inspector** para arrastrar los objetos `PuntoA` y `PuntoB`. Podemos cambiar la velocidad de cada animal sin modificar el código.

### Paso 2. Empezar en A y dirigirnos a B

```csharp
rb.position = puntoA.position;
destino = puntoB;
```

Al comenzar, colocamos al animal en `PuntoA`. La variable `destino` indica **hacia dónde tiene que ir ahora**; inicialmente será `PuntoB`.

### Paso 3. Calcular hacia dónde debe caminar

```csharp
Vector3 direccion = destino.position - rb.position;
direccion.y = 0;
```

Restamos la posición del animal a la posición del destino para obtener la dirección del movimiento.

Ponemos `direccion.y = 0` porque queremos desplazarnos **sobre la plataforma**, sin utilizar el movimiento automático para subir o bajar. La gravedad se sigue encargando del eje vertical.

### Paso 4. Cambiar de destino cuando llegue

```csharp
if (direccion.magnitude < 0.15f)
{
    destino = destino == puntoB ? puntoA : puntoB;
    return;
}
```

`magnitude` es la distancia que queda hasta el destino. Cuando estamos suficientemente cerca, cambiamos al otro punto.

La línea con `?` y `:` significa lo mismo que escribir:

```csharp
if (destino == puntoB)
    destino = puntoA;
else
    destino = puntoB;
```

Así conseguimos el recorrido **A → B → A → B...**.

### Paso 5. Mover y girar al animal

```csharp
direccion.Normalize();

rb.linearVelocity = new Vector3(
    direccion.x * velocidad,
    rb.linearVelocity.y,
    direccion.z * velocidad
);

rb.MoveRotation(Quaternion.LookRotation(direccion));
```

`Normalize()` deja la dirección con longitud 1 para poder multiplicarla por la velocidad elegida.

Modificamos la velocidad horizontal del **Rigidbody**, pero conservamos su velocidad vertical (`rb.linearVelocity.y`), para no anular la gravedad.

Por último, `MoveRotation` hace que el animal mire hacia donde camina.

### Código completo

Crea `Assets/Scripts/MovimientoAnimales.cs` y pega este código:

```csharp
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovimientoAnimales : MonoBehaviour
{
    // Podemos usar este script en cualquier animal, no solo en el Corgi.
    public Transform puntoA;
    public Transform puntoB;
    public float velocidad = 2f;
    public bool usarAnimaciones = false;

    private Rigidbody rb;
    private Animator animator;
    private Transform destino;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();

        // Es necesario asignar los dos puntos en el Inspector.
        if (puntoA == null || puntoB == null)
        {
            enabled = false;
            return;
        }

        // Empezamos en A y vamos hacia B.
        rb.position = puntoA.position;
        destino = puntoB;

        if (usarAnimaciones && animator != null)
            animator.SetTrigger("Run");
    }

    void FixedUpdate()
    {
        if (destino == null) return;

        // Calculamos hacia dónde debe desplazarse.
        Vector3 direccion = destino.position - rb.position;
        direccion.y = 0; // La gravedad sigue funcionando en el eje Y.

        // Al llegar a B vamos a A, y al llegar a A volvemos a B.
        if (direccion.magnitude < 0.15f)
        {
            destino = destino == puntoB ? puntoA : puntoB;
            return;
        }

        direccion.Normalize();

        // Movemos al animal sin modificar su velocidad vertical.
        rb.linearVelocity = new Vector3(
            direccion.x * velocidad,
            rb.linearVelocity.y,
            direccion.z * velocidad
        );

        // Giramos al animal hacia donde está caminando.
        rb.MoveRotation(Quaternion.LookRotation(direccion));
    }
}
```

Añade el componente `MovimientoAnimales` al animal y asigna `PuntoA` y `PuntoB` en el Inspector. Coloca los puntos a una altura adecuada para que el animal no aparezca bajo el suelo.

Si el modelo tiene animaciones y un trigger `Run`, activa `Usar Animaciones`. En el Animator, el clip de correr debe estar en bucle y la transición `Idle → Run` no debe esperar a `Has Exit Time`. Desactiva también **Apply Root Motion**.

## 3. Añadir el arrastre al animal

No necesitamos escribir otro script: reutilizamos **ArrastrarFigura3D.cs** del ejercicio 2.

**[Ver el código completo y la explicación paso a paso](ejercicio-2-figuras.md#3-script-arrastrarfigura3dcs)** · [Abrir archivo .cs](https://github.com/erciapps/web-home/blob/main/docs/pmdm/scripts/ArrastrarFigura3D.cs)

Añade los **dos componentes al mismo animal**:

- `MovimientoAnimales`, para que corra de A a B.
- `ArrastrarFigura3D`, para poder cogerlo y soltarlo.

En el Inspector de **ArrastrarFigura3D**, arrastra su componente **MovimientoAnimales** al campo **Movimiento Automatico**. Así un script podrá pausar al otro.

### ¿Qué pasa cuando pulsamos sobre el animal?

1. `ArrastrarFigura3D` lanza un **Raycast**, un rayo invisible desde la cámara, para comprobar si hemos tocado su Collider.
2. **Solo si hemos tocado ese animal**, desactivamos temporalmente `MovimientoAnimales` con `movimientoAutomatico.enabled = false`.
3. Mientras lo tenemos cogido, detenemos sus físicas y lo desplazamos sobre un plano horizontal.
4. Cuando lo soltamos, reactivamos las físicas y `MovimientoAnimales`. El animal vuelve a caminar hacia su destino.

No hace falta que cada uno de los scripts conozca el nombre del otro: el campo `Movimiento Automatico` admite el componente que asignemos desde el Inspector. Por eso podemos reutilizar `ArrastrarFigura3D` también con objetos que no patrullan.

Si queremos las animaciones de `Run` e `Idle` al coger y soltar, activa `Usar Animaciones` en el script de arrastre **solo si esos triggers existen**. Si el animal sigue patrullando después de soltarlo, continúa en `Run`.

## 4. Probarlo con varios animales

Podemos repetir el proceso con todos los animales, asignando una velocidad diferente y puntos de origen y destino. Por ejemplo, con cinco animales podemos utilizar **cinco parejas de puntos (diez puntos en total)** sin cambiar el script.

De momento cada animal recorre su tramo **A↔B**. Más adelante podemos modificar el recorrido para compartir una lista de diez puntos y elegir destinos aleatorios.

!!! tip "Si algo no funciona"
    Si el animal camina pero no puedes seleccionarlo, comprueba **Box Collider → Edit Collider**. Si puedes cogerlo pero continúa moviéndose solo, comprueba que has asignado su componente `MovimientoAnimales` al campo `Movimiento Automatico` del script de arrastre. Si el animal se cae, revisa el Collider de la plataforma y `Use Gravity`.

## Próxima ampliación

Cuando terminemos de colocar los animales y las tres cestas, añadiremos la lógica para reconocer el grupo de cada animal (**mamíferos, reptiles o aves**), comprobar dónde lo soltamos y mostrar un acierto o un error.

[← Ejercicio 2 · Clasificador de figuras](ejercicio-2-figuras.md) · [Volver a PMDM](index.md)
