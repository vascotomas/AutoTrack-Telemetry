using AutoTelemetryEntities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoTelemetryWorker.Interfaces
{
    public interface ITelemetryNotifier
    {
        Task NotifyStatusChangedAsync(Guid id, EstadoTelemetria nuevoEstado, CancellationToken ct);
    }
}
