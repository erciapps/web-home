# Ejercicio 5 · Laberinto 3D controlado por voz

Vamos a crear un minijuego en **Unity 6 para Android**. Un personaje 3D de KayKit debe salir de un laberinto formado por casillas. Lo controlaremos **únicamente con órdenes de voz**. No habrá botones de movimiento.

## 1. Recursos y proyecto

Crea un proyecto **Universal 3D (URP)**. Descarga los modelos desde itch.io:

- [KayKit Adventurers](https://kaylousberg.itch.io/kaykit-adventurers): personaje 3D.
- [KayKit Prototype Bits](https://kaylousberg.itch.io/prototype-bits): objetos estáticos para los obstáculos.

Importa los modelos en Assets. Los recursos se descargan desde sus páginas originales; no se incluyen en este repositorio.

## 2. Instalar el reconocimiento de voz

En Unity abre **Window → Package Manager → + → Install package from git URL** y pega:

~~~text
https://github.com/yasirkula/UnitySpeechToText.git
~~~

El paquete se llama **Speech to Text** y utiliza el reconocimiento de voz del dispositivo. En Android necesita permiso de micrófono; el script lo solicitará cuando pulsemos el botón.

!!! note
    El reconocimiento no es continuo: una pulsación inicia una sesión, el usuario habla y el sistema devuelve el texto definitivo. En algunos móviles requiere que el servicio de reconocimiento de voz esté disponible y conexión a Internet.

## 3. Crear el tablero 3D

Crea y guarda la escena **LaberintoVoz**.

1. Crea un objeto vacío **Tablero**.
2. Dentro crea **Cube** de escala **(1, 0.2, 1)**, centrado en **(0, -0.1, 0)**. Su cara superior quedará en Y = 0.
3. Duplica el cubo para obtener un tablero de **8 × 8 casillas**. Sus centros deben estar en coordenadas enteras: X = 0 a 7, Z = 0 a 7.
4. Añade varios objetos estáticos de **KayKit Prototype Bits** sobre las casillas que quieras bloquear.
5. A cada obstáculo añádele **Box Collider** (ajústalo a su tamaño). Crea la capa **Obstaculos** y asígnala a esos objetos.
6. Marca una casilla libre como **Meta** con un objeto visible. Su posición X y Z también debe ser entera.

!!! warning "Importante"
    Los obstáculos tienen Collider, **pero no Rigidbody**, porque son estáticos. No asignes la capa Obstaculos al tablero ni a la meta. La posición lógica del personaje se comprobará antes de avanzar a cada casilla, para evitar atravesar objetos.

### Preparar el personaje

1. Crea un GameObject vacío llamado **Jugador** y colócalo inicialmente en **(0, 0, 0)**, o en otra casilla libre de coordenadas enteras.
2. Añade el modelo 3D de KayKit como **hijo** de Jugador. Ajusta su escala y posición local para que sus pies toquen el suelo.
3. La orientación inicial de Jugador será **Rotation (0, 0, 0)**; mirará hacia **Z positivo**.
4. No es necesario añadir Rigidbody ni Character Controller: el script se encargará del desplazamiento por casillas.

Usa una cámara fija elevada que permita ver el tablero completo.

## 4. Instrucciones permitidas

| Lo que dice el jugador | Acción |
| --- | --- |
| **Avanza cinco casillas** | Se desplaza 5 casillas en la dirección a la que mira |
| **Retrocede cinco casillas** | Retrocede 5 casillas sin cambiar la orientación |
| **Gira izquierda** | Gira 90° a la izquierda |
| **Gira derecha** | Gira 90° a la derecha |
| **Da la vuelta** | Gira 180° en sentido horario |

También se aceptan el número **5** en lugar de «cinco» y la frase «gira a la derecha/izquierda».

El usuario puede decir una instrucción o encadenar varias con **«y»**, por ejemplo:

~~~text
Avanza cinco casillas y gira derecha y retrocede cinco casillas
~~~

El script transforma el texto final en un **array de instrucciones** y las ejecuta en orden. Las palabras que no pertenezcan a las instrucciones válidas provocan el rechazo de toda la frase. Si una casilla está ocupada, se detiene la secuencia. Se gana al llegar a la meta.

## 5. Script LaberintoVoz.cs

Crea **Assets/Scripts/LaberintoVoz.cs** y copia este archivo:

**[Descargar / ver LaberintoVoz.cs](scripts/LaberintoVoz.cs)**

El script contiene:

- Una referencia al **Jugador** y otra a la **Meta**.
- Un **LayerMask** para comprobar qué casillas están ocupadas.
- Un array **instrucciones** visible en el Inspector durante Play.
- El reconocimiento de voz mediante **ISpeechToTextListener**.
- La ejecución secuencial: giros de 90° / 180° y desplazamientos de cinco casillas.

Solo se interpretan los resultados finales de voz (**OnResultReceived**), no el texto provisional (**OnPartialResultReceived**). Así evitamos ejecutar dos veces una misma orden.

## 6. Preparar la interfaz

En un Canvas añade:

- Un **Text - TextMeshPro** llamado **TextoOrdenes**.
- Un botón **ESCUCHAR**.

Crea un GameObject vacío **Controlador**, añádele el script LaberintoVoz.cs y configura sus campos:

| Campo | Referencia / valor |
| --- | --- |
| Jugador | Arrastra el objeto Jugador |
| Meta | Arrastra el objeto Meta |
| Texto | TextoOrdenes |
| Obstaculos | Selecciona únicamente la capa Obstaculos |
| Columnas | 8 |
| Filas | 8 |
| Velocidad | 3 |

En el botón **ESCUCHAR → On Click()**, arrastra **Controlador** y selecciona **LaberintoVoz → Escuchar()**.

## 7. Probar en Android

Abre **File → Build Profiles → Android** y utiliza **Build And Run**. Concede permiso de micrófono cuando se solicite.

Prueba las cinco instrucciones. Comprueba que:

1. El array recoge únicamente las órdenes reconocidas y válidas.
2. El personaje recorre **cinco** casillas por cada «avanza» o «retrocede».
3. Los giros modifican la orientación, pero no la casilla actual.
4. No puede atravesar obstáculos ni salir del tablero.
5. Al llegar a la meta aparece **¡META CONSEGUIDA!**

!!! note "Pruebas en el editor"
    El plugin de voz utiliza un simulador en Unity Editor; para reconocer de verdad lo que decimos hay que ejecutar la aplicación en Android.

## Entrega

Una escena funcional con tablero 8 × 8, personaje KayKit, obstáculos estáticos, meta, botón para escuchar y los cinco comandos de voz. El juego debe impedir movimientos fuera del tablero o hacia casillas bloqueadas.
