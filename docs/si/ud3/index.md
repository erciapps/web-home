# UD3 · Redes TCP/IP y conectividad

En esta unidad aprenderás a conectar equipos en red, configurar sus direcciones IP y comprobar que pueden comunicarse. Empezaremos creando redes locales con **GNS3** y, más adelante, utilizaremos routers MikroTik CHR para trabajar con puertas de enlace, DHCP, NAT y puertos.

## Objetivos de la unidad

- Identificar los elementos de una red local y comprender la función de un switch y de un router.
- Configurar direcciones IPv4, máscaras de subred y puertas de enlace.
- Comprender el funcionamiento básico de las direcciones MAC y del protocolo ARP.
- Comprobar la conectividad con comandos de diagnóstico.
- Configurar progresivamente servicios y acceso entre redes.


## Material teórico

[**Subredes, bridges y VLANs (PDF)**](SUBNETING2026.pdf)

Apuntes sobre máscaras de red, subnetting, bridges y segmentación mediante VLANs, con ejemplos y esquemas.

## Direcciones WAN y puertos asignados

Utiliza exclusivamente la IP WAN estática y los puertos externos asignados a tu usuario. La máscara WAN es `/24` y la puerta de enlace y DNS son `192.168.122.1`. No utilices los puertos de otros alumnos.

| Alumno | IP WAN | Puertos asignados |
| --- | --- | --- |
| dam70 | `192.168.122.70` | 20000–20010 |
| dam93 | `192.168.122.93` | 20011–20021 |
| dam94 | `192.168.122.94` | 20022–20032 |
| dam96 | `192.168.122.96` | 20033–20043 |
| dam98 | `192.168.122.98` | 20044–20054 |
| dam99 | `192.168.122.99` | 20055–20065 |
| dam100 | `192.168.122.100` | 20066–20076 |
| dam82 | `192.168.122.82` | 20077–20087 |
| dam104 | `192.168.122.104` | 20088–20098 |
| dam87 | `192.168.122.87` | 20099–20109 |
| dam88 | `192.168.122.88` | 20110–20120 |
| dam90 | `192.168.122.90` | 20121–20131 |
| dam107 | `192.168.122.107` | 20132–20142 |
| dam108 | `192.168.122.108` | 20143–20153 |
| dam77 | `192.168.122.77` | 20154–20164 |
| dam79 | `192.168.122.79` | 20165–20175 |
| damx | `192.168.122.120` | 20176–20186 |

## Ejercicios

<div class="si-units">
  <article class="si-card available" style="--accent:#ff7a00">
    <div class="si-card-top"><span class="si-code">UD3.1</span><span class="si-state available">Disponible</span></div>
    <div class="si-unit-icon">LAN</div>
    <h3>Creación de una red local con GNS3</h3>
    <p>Conecta tres ordenadores a un switch, configura direcciones IP personalizadas, comprueba la comunicación y descubre ARP y la tabla MAC.</p>
    <div class="si-card-bottom"><div class="si-ra"><span>GNS3 · VPCS · IPv4 · ping · ARP</span></div><div class="si-actions"><a class="si-button" href="ejercicios/ud3_1/">Ver ejercicio <span>→</span></a></div></div>
  </article>
  <article class="si-card available" style="--accent:#ff7a00">
    <div class="si-card-top"><span class="si-code">UD3.2</span><span class="si-state available">Disponible</span></div>
    <div class="si-unit-icon">WAN</div>
    <h3>MikroTik, DHCP, Webterm y acceso a Internet</h3>
    <p>Amplía tu LAN con un router MikroTik, configura DHCP desde WinBox y compara tablas ARP.</p>
    <div class="si-card-bottom"><div class="si-ra"><span>GNS3 · MikroTik · DHCP · Webterm · ARP</span></div><div class="si-actions"><a class="si-button" href="ejercicios/ud3_2/">Ver ejercicio <span>→</span></a></div></div>
  </article>
  <article class="si-card available" style="--accent:#ff7a00">
    <div class="si-card-top"><span class="si-code">UD3.3</span><span class="si-state available">Disponible</span></div>
    <div class="si-unit-icon">SW</div>
    <h3>Administración de un switch MikroTik RouterOS</h3>
    <p>Configura un bridge desde WinBox, consulta la tabla MAC y administra los puertos del switch.</p>
    <div class="si-card-bottom"><div class="si-ra"><span>GNS3 · MikroTik · WinBox · Bridge · MAC</span></div><div class="si-actions"><a class="si-button" href="ejercicios/ud3_3/">Ver ejercicio <span>→</span></a></div></div>
  </article>
  <article class="si-card available" style="--accent:#ff7a00">
    <div class="si-card-top"><span class="si-code">UD3.4</span><span class="si-state available">Disponible</span></div>
    <div class="si-unit-icon">TCP</div>
    <h3>Servidor web y redirección de puertos con MikroTik</h3>
    <p>Publica un servidor Python de la LAN, configura la redirección desde WinBox y comprueba el acceso desde la WAN.</p>
    <div class="si-card-bottom"><div class="si-ra"><span>GNS3 · MikroTik · NAT · TCP · Python</span></div><div class="si-actions"><a class="si-button" href="ejercicios/ud3_4/">Ver ejercicio <span>→</span></a></div></div>
  </article>
</div>

!!! tip "Entorno de trabajo"
    Accede al servidor GNS3 con tu usuario. Cada alumno debe crear su propio proyecto y utilizar la red indicada por su identificador `damXX`.
