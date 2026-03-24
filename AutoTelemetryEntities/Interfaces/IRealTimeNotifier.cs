using AutoTelemetryEntities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoTelemetryEntities.Interfaces
{
    public interface IRealTimeNotifier
    {
        Task BroadcastEventAsync<TPayload>(string eventName, Guid transactionId, TPayload payload, CancellationToken ct);
    }
}
