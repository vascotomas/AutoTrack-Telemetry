using AutoTelemetryEntities.Enums;
using AutoTelemetryEntities.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace AutoTelemetryInfrastructure.NotificationsTelemetry
{
    public class SignalRNotifier : IRealTimeNotifier
    {
        private readonly IHubContext<HubTelemetry> _hubContext;

        public SignalRNotifier(IHubContext<HubTelemetry> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task BroadcastEventAsync<TPayload>(string eventName, Guid transactionId, TPayload payload, CancellationToken ct)
        {
            await _hubContext.Clients.All.SendAsync(eventName, transactionId, payload, cancellationToken: ct);
        }

    }
}

