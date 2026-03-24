using AutoTelemetryEntities.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoTelemetryEntities.Interfaces
{
    public interface ITelemetryRepository
    {
        Task<bool> ExisteChasisAsync(string chasisId, CancellationToken ct);
        Task GuardarEventoAsync(TelemetryEvent evento, CancellationToken ct);
    }
}
