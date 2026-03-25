# 🏭 AutoTrack Telemetry

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp)
![SignalR](https://img.shields.io/badge/SignalR-RealTime-0078D4?style=for-the-badge)
![SQLite](https://img.shields.io/badge/SQLite-003B57?style=for-the-badge&logo=sqlite)
![Blazor](https://img.shields.io/badge/Blazor-Web-512BD4?style=for-the-badge&logo=blazor)
![WinForms](https://img.shields.io/badge/WinForms-Desktop-5C2D91?style=for-the-badge)

Sistema de **telemetría industrial orientado a eventos**. Diseñado para la ingesta de alto volumen de lecturas de sensores, procesamiento asíncrono mediante colas en memoria y actualización de tableros de control en **tiempo real**.

La arquitectura está pensada para entornos de manufactura, garantizando la persistencia de datos, el manejo de concurrencia y la notificación instantánea a los operarios de planta a través de múltiples plataformas (Escritorio y Web).

---

## 🏗️ Arquitectura del Sistema

La solución implementa una separación estricta por responsabilidades (Clean Architecture adaptada) para garantizar escalabilidad y mantenimiento.

* Diagrama WinForms
<img width="851" height="532" alt="image" src="https://github.com/user-attachments/assets/52e84e00-e6dc-41f2-a639-d7010bc7a113" />

* Diagrama Blazor Web App
<img width="2105" height="636" alt="image" src="https://github.com/user-attachments/assets/8f3e77b2-f377-4173-9ff3-807d52265eb1" />

---

## 🧩 Componentes Principales

| Capa | Proyecto | Rol Principal |
|------|----------|---------------|
| **Presentación (Escritorio)** | `AutoTelemetryUI` | Panel de control clásico (WinForms). Login, envío de lecturas y monitoreo en vivo. |
| **Presentación (Web)** | `AutoTelemetry.Blazor` | Interfaz web moderna que replica la experiencia del panel de control, accesible desde el navegador. |
| **SDK** | `ClientSDK` | Librería compartida por ambos clientes (WinForms y Blazor). Encapsula llamadas HTTP (con resiliencia vía **Polly**) y conexión SignalR. |
| **API REST** | `AutoTelemetry.API` | Puerta de enlace. Ingesta de datos, validaciones (**FastEndpoints**), Rate Limiting y Host de SignalR. |
| **Procesamiento** | `AutoTelemetryWorker` | `BackgroundService` concurrente. Desencola, aplica reglas de negocio y persiste los datos. |
| **Infraestructura** | `AutoTelemetryInfrastructure` | Acceso a datos con **EF Core**, manejo de transacciones y emisión de eventos en tiempo real. |
| **Dominio** | `AutoTelemetryEntities` | Entidades puras, Enums, Excepciones de negocio y Contratos (Interfaces). |

---

## ⚡ Flujo Funcional (Lifecycle)

1. **Autenticación:** El cliente (WinForms o Blazor) inicia sesión contra un servidor de identidad (OAuth2/OIDC) utilizando el flujo *Password Grant* para obtener un `access_token`.
2. **Ingesta:** El operario envía una lectura del chasis. El SDK lo transmite a la API (`POST /api/telemetry`).
3. **Encolado (Backpressure):** La API valida el payload estructuralmente y lo empuja a un **Canal acotado en memoria** (`Channel<T>`), liberando la petición HTTP inmediatamente (respuesta ultra rápida).
4. **Procesamiento Asíncrono:** Los *JobWorkers* toman los mensajes del canal, validan la integridad lógica (ej. prevención de duplicados vía índice único), evalúan reglas térmicas y persisten en **SQLite**.
5. **Broadcasting:** Se emite el evento `TelemetryProcessed` a través de **SignalR**.
6. **Reacción UI:** Los paneles activos (tanto en WinForms como en navegadores vía Blazor) interceptan el evento y actualizan sus grillas de monitoreo en tiempo real, aplicando semántica de colores según el estado final.

---

## 🔐 Seguridad y Configuración

El sistema está protegido mediante **JWT (JSON Web Tokens)**. La API confía en un servidor de autorización externo para validar las firmas.

**Variables de Entorno / Configuración:**

* **API (`appsettings.json`):**
    Requiere apuntar a la URL de la Autoridad que emite los tokens.
    ```json
    {
      "Authentication": {
        "Authority": "http://localhost:7173"
      }
    }
    ```

* **Cliente WinForms (`App.config`):**
    Requiere las rutas de los servicios y credenciales para negociar el token.
    ```xml
    <appSettings>
      <add key="AuthServerUrl" value="http://localhost:7173" />
      <add key="AuthClientSecret" value="TU_SECRETO_AQUI" />
      <add key="ApiUrl" value="https://localhost:7105" />
    </appSettings>
    ```

* **Cliente Blazor (`wwwroot/appsettings.json` o equivalente):**
    Configuración para la conexión con el servidor de identidad y la API.
    ```json
    {
      "Oidc": {
        "Authority": "http://localhost:7173",
        "ClientId": "BlazorClient"
      },
      "ApiSettings": {
        "BaseUrl": "https://localhost:7105"
      }
    }
    ```

---

## 🔭 Observabilidad

El sistema integra Serilog a lo largo de todas las capas para proveer trazabilidad estructurada.

1. Logs de consola enriquecidos para desarrollo.
2. Trazabilidad de subprocesos (Thread IDs) para monitorear el paralelismo del Worker.
3. Intercepción de peticiones HTTP en la API.

---

## 🚀 Guía de Inicio Rápido (Local)

### 1. Certificados de Desarrollo (SSL/TLS)
Para que los microservicios puedan comunicarse de forma segura localmente sin arrojar errores de red, es necesario confiar en el certificado de desarrollo de .NET.
Abrir una terminal (CMD o PowerShell) y ejecutar el siguiente comando:
```bash
dotnet dev-certs https --trust
```
### 2. Levantar el Servidor de Autenticación
La API y los clientes dependen de un servidor de identidad que debe iniciarse primero.

Clonar el repositorio del AuthServer: vascotomas/AuthorizationServer

Compilá y ejecutá el proyecto del AuthServer (asegurate de que esté corriendo en el puerto esperado,  ej: http://localhost:7173).

### 3. Configurar y Ejecutar AutoTrack Telemetry

Como el sistema requiere que el Backend (API) y el Frontend (UI) corran al mismo tiempo, la mejor forma de ejecutarlo en Visual Studio es configurando los **Múltiples proyectos de inicio**. 

Sigue estos pasos para dejar tu entorno listo:

1. Hacé clic derecho sobre la **Solución "AutoTelemetry"** (el primer elemento hasta arriba) en el Explorador de Soluciones.
2. Seleccioná **"Configurar proyectos de inicio..."** (Configure Startup Projects).
3. En la ventana que se abre, elegí la opción **"Varios proyectos de inicio:"** (Multiple startup projects).
4. Configura las acciones de la grilla según el entorno que quieras probar:

#### 🌐 Opción A: Entorno Web (Blazor + API)
Ideal para probar la experiencia multiplataforma directamente en el navegador. En la grilla, configura las acciones así:
* `AutoTelemetryBlazor` ➡️ Acción: **Inicio** (Destino: http)
* `AutoTelemetryAPI` ➡️ Acción: **Inicio** (Destino: https)
* *El resto de los proyectos deben quedar en "Ninguno".*

#### 🖥️ Opción B: Entorno de Escritorio (WinForms + API)
Ideal para probar el panel de control industrial clásico. En la grilla, cambia las acciones a:
* `AutoTelemetryUI` ➡️ Acción: **Inicio** *
* `AutoTelemetryAPI` ➡️ Acción: **Inicio** (Destino: https)
* *El resto de los proyectos deben quedar en "Ninguno".*

5. Hacé clic en **Aplicar** y luego en **Aceptar**.
6. Finalmente, presioná `F5` (o el botón "Iniciar" en la barra superior de Visual Studio) para arrancar la simulación conjunta. Logueate con las credenciales de prueba `admin` , `password123` y comenzá a transmitir.
