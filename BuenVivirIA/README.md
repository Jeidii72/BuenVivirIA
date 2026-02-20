## BuenVivirIA

BuenVivirIA es una aplicación web que utiliza Inteligencia Artificial para analizar 
la viabilidad de reubicación internacional según el perfil del usuario.

El sistema evalúa factores como:

- País de interés
- Nivel de presupuesto
- Tipo de trabajo
- Prioridad personal (seguridad, costo de vida, etc.)

Y genera un análisis estructurado con:

- Puntaje de viabilidad (0–100)
- Nivel (Baja, Media, Alta)
- Ciudad recomendada
- Ventajas
- Desventajas
- Insight estratégico

##  Tecnologías utilizadas

- ASP.NET Core Web API
- OpenRouter (Modelo Mistral 7B)
- System.Text.Json
- HTML + CSS + JavaScript
- Swagger (documentación API

##  ¿Cómo funciona?

1. El usuario llena el formulario en el frontend.
2. Se envía un `POST` a la API.
3. El backend:
   - Construye un prompt estructurado.
   - Envía la solicitud a OpenRouter.
   - Limpia el JSON recibido.
   - Normaliza el puntaje (escala 0–100).
   - Corrige el nivel automáticamente si es necesario.
4. Devuelve un objeto estructurado al frontend.
5. El frontend muestra el resultado con estilo visual.
{
  "pais": "Portugal",
  "nivelPresupuesto": "Medio",
  "tipoTrabajo": "Remoto",
  "prioridad": "Seguridad"
}


##  Escala de evaluación

- 0 – 39 → 🔴 Baja
- 40 – 69 → 🟡 Media
- 70 – 100 → 🟢 Alta

Si la IA devuelve escala 1–10, el sistema la normaliza automáticamente a 0–100.

---
## Errores 
Error 401 Unauthorized

Causa: API Key incorrecta o vencida.
Solución: Verificar configuración en appsettings.json.


## Configuración
https://github.com/Jeidii72/BuenVivirIA.git
En `appsettings.json` debes agregar tu API Key:
```json
"OpenRouter": {
  "ApiKey": "TU_API_KEY"
}

