using AutoTelemetryWorker.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace AutoTelemetryAPI.Features.NotificationsTelemetry
{
    public class SignalRNotifier : ITelemetryNotifier
    {
        private readonly IHubContext<HubTelemetry> _hubContext;

        public SignalRNotifier(IHubContext<HubTelemetry> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyStatusChangedAsync(string chasisId, string nuevoEstado, CancellationToken ct) 
        => await _hubContext.Clients.All.SendAsync("TelemetryProcessed", chasisId, nuevoEstado, cancellationToken: ct);
        
    }
}

