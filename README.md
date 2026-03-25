# 🏭 AutoTrack Telemetry

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp)
![SignalR](https://img.shields.io/badge/SignalR-RealTime-0078D4?style=for-the-badge)
![SQLite](https://img.shields.io/badge/SQLite-003B57?style=for-the-badge&logo=sqlite)
![WinForms](https://img.shields.io/badge/WinForms-Desktop-5C2D91?style=for-the-badge)

Sistema de **telemetría industrial orientado a eventos**. Diseñado para la ingesta de alto volumen de lecturas de sensores, procesamiento asíncrono mediante colas en memoria y actualización de tableros de control en **tiempo real**.

La arquitectura está pensada para entornos de manufactura, garantizando la persistencia de datos, el manejo de concurrencia y la notificación instantánea a los operarios de planta.

---

## 🏗️ Arquitectura del Sistema

La solución implementa una separación estricta por responsabilidades (Clean Architecture adaptada) para garantizar escalabilidad y mantenimiento.

<img width="851" height="532" alt="image" src="https://github.com/user-attachments/assets/52e84e00-e6dc-41f2-a639-d7010bc7a113" />

---

## 🧩 Componentes Principales

| Capa | Proyecto | Rol Principal |
|------|----------|---------------|
| **Presentación** | `AutoTelemetryUI` | Panel de control (WinForms). Login, envío de lecturas y monitoreo en vivo (SignalR). |
| **SDK** | `ClientSDK` | Librería compartida. Encapsula llamadas HTTP (con resiliencia vía **Polly**) y conexión SignalR. |
| **API REST** | `AutoTelemetry.API` | Puerta de enlace. Ingesta de datos, validaciones (**FastEndpoints**), Rate Limiting y Host de SignalR. |
| **Procesamiento** | `AutoTelemetryWorker` | `BackgroundService` concurrente. Desencola, aplica reglas de negocio y persiste los datos. |
| **Infraestructura** | `AutoTelemetryInfrastructure` | Acceso a datos con **EF Core**, manejo de transacciones y emisión de eventos en tiempo real. |
| **Dominio** | `AutoTelemetryEntities` | Entidades puras, Enums, Excepciones de negocio y Contratos (Interfaces). |

---

## ⚡ Flujo Funcional (Lifecycle)

1. **Autenticación:** El cliente inicia sesión contra un servidor de identidad (OAuth2/OIDC) utilizando el flujo *Password Grant* para obtener un `access_token`.
2. **Ingesta:** El operario envía una lectura del chasis. El SDK lo transmite a la API (`POST /api/telemetry`).
3. **Encolado (Backpressure):** La API valida el payload estructuralmente y lo empuja a un **Canal acotado en memoria** (`Channel<T>`), liberando la petición HTTP inmediatamente (respuesta ultra rápida).
4. **Procesamiento Asíncrono:** Los *JobWorkers* toman los mensajes del canal, validan la integridad lógica (ej. prevención de duplicados vía índice único), evalúan reglas térmicas y persisten en **SQLite**.
5. **Broadcasting:** Se emite el evento `TelemetryProcessed` a través de **SignalR**.
6. **Reacción UI:** El panel de WinForms intercepta el evento y actualiza la grilla de monitoreo en tiempo real, aplicando semántica de colores según el estado final.

---

## 🔐 Seguridad y Configuración

El sistema está protegido mediante **JWT (JSON Web Tokens)**. La API confía en un servidor de autorización externo para validar las firmas.

** Variables de Entorno / Configuración

**API (`appsettings.json`):**
Requiere apuntar a la URL de la Autoridad que emite los tokens.
```json
{
  "Authentication": {
    "Authority": "http://localhost:7173"
  }
}
```
** Cliente WinForms (`App.config`)

Requiere las rutas de los servicios y el secreto del cliente para negociar el token.

```xml
<appSettings>
  <add key="AuthServerUrl" value="http://localhost:7173" />
  <add key="AuthClientSecret" value="TU_SECRETO_AQUI" />
  <add key="ApiUrl" value="https://localhost:7105" />
</appSettings>
```
---

## 🔭 Observabilidad
 El sistema integra Serilog a lo largo de todas las capas para proveer trazabilidad estructurada.

1. Logs de consola enriquecidos para desarrollo.

2. Trazabilidad de subprocesos (Thread IDs) para monitorear el paralelismo del Worker.

3. Intercepción de peticiones HTTP en la API.
