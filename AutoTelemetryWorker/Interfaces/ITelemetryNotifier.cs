using System;
using System.Collections.Generic;
using System.Text;

namespace AutoTelemetryWorker.Interfaces
{
    public interface ITelemetryNotifier
    {
        Task NotifyStatusChangedAsync(string chasisId, string nuevoEstado, CancellationToken ct);
    }
}
