# Markdown · Guía práctica

Markdown es un lenguaje de marcado ligero que utilizaremos para crear **README**, documentación de proyectos y apuntes de forma sencilla. Los archivos Markdown suelen utilizar la extensión `.md`.

---

## 1. Encabezados

Se utilizan almohadillas `#`. Cuantas más almohadillas, menor nivel del encabezado.

```markdown
# Encabezado 1
## Encabezado 2
### Encabezado 3
#### Encabezado 4
##### Encabezado 5
###### Encabezado 6
```

Ejemplo renderizado:
# Encabezado 1
## Encabezado 2
### Encabezado 3
#### Encabezado 4
##### Encabezado 5
###### Encabezado 6

---

## 2. Énfasis

Puedes dar formato al texto con negrita, cursiva o tachado.

```markdown
*Texto en cursiva*
**Texto en negrita**
~~Texto tachado~~
```
Ejemplo renderizado:

*Texto en cursiva*  
**Texto en negrita**  
~~Texto tachado~~

---

## 3. Listas

### Lista no ordenada:
```markdown
- Elemento 1
- Elemento 2
  - Sub-elemento
```
Ejemplo renderizado:
- Elemento 1
- Elemento 2
  - Sub-elemento

### Lista ordenada:
```markdown
1. Paso uno
2. Paso dos
3. Paso tres
```
Ejemplo renderizado:
1. Paso uno
2. Paso dos
3. Paso tres

---

## 4. Enlaces

```markdown
[ERCI Apps](https://erciapps.es)
```
Ejemplo renderizado: [ERCI Apps](https://erciapps.es)

---

## 5. Imágenes

```markdown
![Texto alternativo](https://via.placeholder.com/150)
```
Ejemplo renderizado:  
![Texto alternativo](https://via.placeholder.com/150)

---

## 6. Código

### En línea:
```markdown
Este es un comando: `ls -la`
```

Ejemplo renderizado: Este es un comando: `ls -la`

### Bloques de código:
````markdown
```python
def hola():
    print("Hola, Markdown!")
```
`````

Ejemplo renderizado:

```python
def hola():
    print("Hola, Markdown!")
```

---

## 7. Citas

```markdown
> Esto es una cita.
```
Ejemplo renderizado:

> Esto es una cita.

---

## 8. Tablas

```markdown
| Columna 1 | Columna 2 |
|-----------|-----------|
| Dato 1    | Dato 2    |
| Dato 3    | Dato 4    |
```
Ejemplo renderizado:

| Columna 1 | Columna 2 |
|-----------|-----------|
| Dato 1    | Dato 2    |
| Dato 3    | Dato 4    |

---

## 9. Separadores

```markdown
---
```
Ejemplo renderizado:

---



---

## 10. Combinando elementos

```markdown
# Lista de comandos útiles

1. **Navegar directorios**
   - `cd nombre_directorio`

2. **Ver archivos**
   - `ls -la`

> Recuerda siempre verificar permisos con `ls -l`.
```

Ejemplo renderizado:

# Lista de comandos útiles

1. **Navegar directorios**
   - `cd nombre_directorio`

2. **Ver archivos**
   - `ls -la`

> Recuerda siempre verificar permisos con `ls -l`.




---



