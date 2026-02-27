# Análisis Detallado de Pruebas UIT - Módulo de Reviews

## Resumen Ejecutivo
He analizado las 7 pruebas del módulo de reviews (CU-ReseñarDispositivo) y encontrado **varios problemas importantes** en los assertions, validaciones y lógica de las pruebas.

---

## 1️⃣ TEST: `UC3_2_3_filteringbyBrandandYear`
**Tipo:** Theory (2 casos de prueba)  
**Propósito:** Verificar que se pueden filtrar dispositivos por marca y año

### Casos de Prueba:
```
[Caso 1] Brand=Apple, Color=Negro, Name=iPhone15, Year=2023, Model=iPhone15 → Filter: Apple/2023
[Caso 2] Brand=Samsung, Color=Gris, Name=GalaxyS23, Year=2023, Model=GalaxyS23 → Filter: Samsung/2023
```

### ✅ ANÁLISIS - ESTADO GENERAL: **BUEN STATE**
- El filtrado por marca está bien implementado en `SelectDevicesForReviewPO.SearchDevices()`
- El filtrado por año está bien implementado
- El assert valida correctamente que los resultados de la búsqueda coincide con los esperados usando `CheckListOfDevices()`

### ⚠️ PROBLEMAS IDENTIFICADOS:
**NINGUNO CRÍTICO** - Esta prueba está bien

---

## 2️⃣ TEST: `UC3_4_ModifySelecteDevices`
**Tipo:** Fact  
**Propósito:** Verificar que se pueden eliminar dispositivos del carrito de reseña

### Flujo:
1. Selecciona 2 dispositivos (iPhone15 y GalaxyS23)
2. Elimina GalaxyS23 (deviceName2)
3. Verifica que iPhone15 (deviceName1) sigue en el carrito

### ✅ ESTADO: **CORRECTO**
- La prueba verifica correctamente que `deviceName1` está en el carrito después de hacer ModifyReviewCart (eliminar)
- El `CheckShoppingCart()` valida que el dispositivo existe en el DOM

---

## 3️⃣ TEST: `UC3_5_ReviewButtonNotAvailable`
**Tipo:** Fact  
**Propósito:** Verificar que cuando el carrito está vacío, el botón de revisar no está disponible

### Flujo:
1. Selecciona 1 dispositivo
2. Elimina ese dispositivo del carrito
3. Verifica que el botón Review está deshabilitado

### ⚠️ PROBLEMAS ENCONTRADOS:

**PROBLEMA 1 - Mensaje de Assert incorrecto:**
```csharp
Assert.True(selectDevices.CheckReviewDeviceDisabled(), "You must select at least one device");
```
- El mensaje dice "You must select at least one device" pero esto no está en el código de validación
- El mensaje debería ser informativo pero aquí es engañoso

**PROBLEMA 2 - Lógica de validación confusa:**
- `CheckReviewDeviceDisabled()` intenta validar múltiples formas de deshabilitación:
  - Contenedor oculto (hidden attribute)
  - Botón con atributo disabled
  - Propiedad Enabled = false
- Esto es demasiado complejo - debería verificar UNA FORMA específica

**RECOMENDACIÓN:**
- Simplificar la lógica de `CheckReviewDeviceDisabled()`
- Actualizar el mensaje del assert a: `"Review button must be disabled when cart is empty"`

---

## 4️⃣ TEST: `UC3_6_7_8_9_testingErrorsMandatorydata`
**Tipo:** Theory (4 casos de prueba)  
**Propósito:** Validar errores en datos obligatorios

### Casos de Prueba:

| Caso | ReviewTitle | Country | Comentario | Rating | Error Esperado |
|------|------------|---------|-----------|--------|----------------|
| 1 | ""(vacío) | Spain | "Good device" | 5 | "El título debe tener entre 10 y 50 caracteres" |
| 2 | "Perfecto rendimiento" | ""(vacío) | "Good device" | 5 | "Por favor, ingrese pais" |
| 3 | "Perfecto rendimiento" | Spain | "Good device" | null | ""(sin error) |
| 4 | "Perfecto rendimiento" | Spain | "Very good device" | 5 | ""(sin error) |

### ❌ PROBLEMAS CRÍTICOS ENCONTRADOS:

**PROBLEMA 1 - Error esperado incorrecto (Caso 1):**
```csharp
// Caso 1: ReviewTitle = "" (vacío)
// Error esperado: "El título debe tener entre 10 y 50 caracteres"
```
- ✗ INCORRECTO: El modelo `Review.cs` valida que `ReviewTitle` tiene entre 10-50 caracteres
- ✓ CORRECTO sería: Debería fallar con el error sobre la longitud mínima

**PROBLEMA 2 - Mensaje de error incorrecto (Caso 2):**
```csharp
// Caso 2: Country = "" (vacío)
// Error esperado: "Por favor, ingrese pais"
```
- ✗ INCORRECTO: En el controlador dice `"Error! El pais no puede estar vacio"` no "Por favor, ingrese pais"
- La prueba está buscando el mensaje incorrecto

**PROBLEMA 3 - Rating nulo (Caso 3):**
```csharp
if (rating.HasValue)
{
    createreview.AddDeviceReviewRating(deviceId1, rating.Value);
    // ...
}
```
- La prueba NO llena el rating (null)
- Pero continúa intentando crear la reseña
- ✗ El controlador NO verifica que el rating sea obligatorio en ReviewForCreateDTO
- ⚠️ Esto puede causar un comportamiento indefinido

**PROBLEMA 4 - Comentario no valida el formato:**
- El caso 4 llena: `"Very good device"` como comentario
- ✗ PERO: El controlador (líneas 155-157) valida que el comentario DEBE empezar con `"Reseña para"`
- La prueba debería fallar pero el error esperado es vacío (`""`)
- ✓ Debería esperar error: `"Error! El comentario del dispositivo 'iPhone 15' debe empezar por 'Reseña para'"`

### RECOMENDACIONES:
```csharp
[Theory]
[InlineData("maria@alu.uclm.es", "Spain", "", "Reseña para Good", 5, 
    "El nombre tiene que tener entre 10 y 50 caracteres")] // ReviewTitle vacío
[InlineData("maria@alu.uclm.es", "", "Perfecto rendimiento", "Reseña para Good", 5, 
    "Error! El pais no puede estar vacio")] // Country vacío
[InlineData("maria@alu.uclm.es", "Spain", "Perfecto rendimiento", "Good device", 5, 
    "Error! El comentario del dispositivo 'iPhone 15' debe empezar por 'Reseña para'")]  // Comentario sin "Reseña para"
[InlineData("maria@alu.uclm.es", "Spain", "Perfecto rendimiento", "Reseña para Good", 5, 
    "")]  // Todo correcto - sin error
public void UC3_6_7_8_9_testingErrorsMandatorydata(...)
```

---

## 5️⃣ TEST: `UC3_10_ModifyRentalItems`
**Tipo:** Fact  
**Propósito:** Verificar que se puede volver atrás desde la página de crear reseña para modificar items

### Flujo:
1. Selecciona 1 dispositivo
2. Va a la página de crear reseña
3. Presiona "Modify Devices" (volver atrás)
4. Verifica que el dispositivo sigue en el carrito

### ✅ ESTADO: **CORRECTO**
- La prueba verifica correctamente que `deviceName1` sigue en el carrito
- Simula un flujo de usuario realista

---

## 6️⃣ TEST: `UC3_11_12_testingErrorsvalidationdata`
**Tipo:** Theory (2 casos de prueba)  
**Propósito:** Validar errores en la validación de datos

### Casos de Prueba:

| Caso | ReviewTitle | Country | Comentario | Rating | Error Esperado |
|------|------------|---------|-----------|--------|----------------|
| 1 | "Perfecto rendimiento" | Spain | "Not recommended" | -5 | ""(sin error) |
| 2 | "Perfecto rendimiento" | Spain | "Not recommended" | 5 | ""(sin error) |

### ❌ PROBLEMAS CRÍTICOS:

**PROBLEMA 1 - Rating negativo no genera error (Caso 1):**
```csharp
// Rating = -5
// Expected error: "" (sin error)
```
- ✗ INCORRECTO: El controlador valida (línea 148-150):
```csharp
else if (item.Rating < 1 || item.Rating > 5)
{
    ModelState.AddModelError("ReviewItems", $"Error! La puntuacion...");
}
```
- ✓ Debería esperar error: `"Error! La puntuacion del dispositivo 'iPhone 15' debe estar entre 1 y 5"`

**PROBLEMA 2 - Comentario incorrecto en ambos casos:**
- Ambos casos usan `"Not recommended"` como comentario
- ✗ PERO: El controlador valida que debe empezar por `"Reseña para"`
- La prueba debería fallar en ambos casos
- ✓ Los casos correctos deberían usar: `"Reseña para Good"` o similar

**PROBLEMA 3 - Rating falta en método `AddDeviceReviewRating()`:**
En `CreateReviewPO.cs` líneas 59-64:
```csharp
if (rating < 1 || rating > 5)
{
    _output.WriteLine($"⚠ Rating inválido: {rating}. Rango válido: 1-5. Se omitirá la selección.");
    return; // !!! RETORNA SIN HACER NADA
}
```
- Cuando el rating es -5, el método retorna sin hacer nada
- Pero el test sigue como si nada aconteció - es inconsistente

### RECOMENDACIONES:
```csharp
[Theory]
[InlineData("maria@alu.uclm.es", "Spain", "Perfecto rendimiento", "Reseña para Bad", -5, 
    "Error! La puntuacion del dispositivo 'iPhone 15' debe estar entre 1 y 5")]  // Rating inválido
[InlineData("maria@alu.uclm.es", "Spain", "Perfecto rendimiento", "Reseña para Good", 5, 
    "")] // Todo correcto
[InlineData("maria@alu.uclm.es", "Spain", "Perfecto rendimiento", "Not recommended", 5, 
    "Error! El comentario del dispositivo 'iPhone 15' debe empezar por 'Reseña para'")]  // Comentario sin prefix
public void UC3_11_12_testingErrorsvalidationdata(...)
```

---

## 7️⃣ TEST: `UC2_1_BasicFlow`
**Tipo:** Theory (1 caso de prueba)  
**Propósito:** Flujo completo correcto de crear una reseña

### Datos:
```
Username: maria@alu.uclm.es
Country: Spain
ReviewTitle: "Perfecto rendimiento" (20 caracteres - ✓ válido 10-50)
Comentario: "Reseña para" (10 caracteres - ✓ empieza por "Reseña para")
Rating: 5 (✓ válido 1-5)
```

### ✅ ESTADO: **CORRECTO CON OBSERVACIONES**
- El flujo de navegación es correcto
- Los datos son válidos
- Los assertions validan correctamente:
  - Detalles de la reseña (nombre, título, país)
  - Items de la reseña (nombre, modelo, año, rating, comentario)

### ⚠️ OBSERVACIÓN:
- El comentario es muy corto: "Reseña para" (solo 10 caracteres y no tiene sentido)
- Debería ser algo como: "Reseña para este dispositivo es excelente" (más realista)

---

## 📊 RESUMEN DE PROBLEMAS POR SEVERIDAD

### 🔴 CRÍTICOS (afectan la confiabilidad de las pruebas):
1. **UC3_6_7_8_9** - Casos 1, 2 y 4 tienen errores esperados incorrectos
2. **UC3_11_12** - Ambos casos esperan 0 errores cuando deberían esperar errores
3. **UC3_11_12** - Rating negativo no genera error esperado

### 🟡 MODERADOS (problemas de validación):
1. **UC3_5** - Mensaje de assert incorrecto/engañoso
2. **UC3_5** - Lógica de `CheckReviewDeviceDisabled()` demasiado compleja
3. **UC3_6_7_8_9** - Rating nulo no se valida

### 🟢 BAJOS (mejoras menores):
1. **UC2_1_BasicFlow** - Comentario debería ser más realista

---

## ✅ RECOMENDACIONES FINALES

### Paso 1: Corregir UC3_6_7_8_9
- Arreglar los mensajes de error esperados
- Agregar validación para comentarios que NO empiezan por "Reseña para"
- Verificar minLength para ReviewTitle

### Paso 2: Corregir UC3_11_12
- Cambiar casos para usar comentarios válidos ("Reseña para...")
- Esperar error cuando rating es -5
- Agregar caso para comentario inválido

### Paso 3: Mejoras generales
- Refactorizar `CheckReviewDeviceDisabled()` para ser más simple
- Hacer comentarios más realistas en todas las pruebas
- Agregar más casos de prueba para validar campos con longitud mínima

---

## 📝 CONCLUSIÓN
Las pruebas tienen una **cobertura lógica buena** pero **múltiples problemas en los casos de prueba Theory** 
donde los valores de entrada y errores esperados no coinciden con la lógica real del controlador.

**Recomendación:** Ejecutar las pruebas con los cambios sugeridos para asegurar que realmente validan lo que pretenden validar.
