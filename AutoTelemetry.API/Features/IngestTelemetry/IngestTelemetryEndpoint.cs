using AutoTelemetry.Features.IngestTelemetry;
using AutoTelemetryEntities.Entities;
using AutoTelemetryEntities.Enums;
using FastEndpoints;
using System.Threading.Channels;

namespace AutoTelemetryAPI.Features.IngestTelemetry
{
    public class IngestTelemetryEndpoint : Endpoint<IngestTelemetryRequest, IngestTelemetryResponse>
    {
        private readonly Channel<QueueMessage> _channel;
        private readonly ILogger<IngestTelemetryEndpoint> _logger;

        public IngestTelemetryEndpoint(Channel<QueueMessage> channel, ILogger<IngestTelemetryEndpoint> logger)
        {
            _channel = channel;
            _logger = logger;
        }

        public override void Configure()
        {
            Post("/api/telemetry");
            Options(x => x.RequireRateLimiting("telemetry-policy"));
        }
        public override async Task HandleAsync(IngestTelemetryRequest req, CancellationToken ct)
        {
            var telemetryEvent = new TelemetryEvent
            {
                Id = req.Id,
                ChasisId = req.ChasisId,
                Estacion = req.Estacion,
                Temperatura = req.Temperatura,
                Timestamp = DateTime.UtcNow,
                Estado = EstadoTelemetria.Pendiente
            };
            var msg = new QueueMessage { Job = telemetryEvent };

            await _channel.Writer.WriteAsync(msg, ct);

            _logger.LogInformation("Chasis {ChasisId} encolado exitosamente.", req.ChasisId);

            await Send.OkAsync(new IngestTelemetryResponse { Message = "Evento recibido y encolado para procesamiento.", ChasisId = req.ChasisId }, cancellation: ct);

        }
    }
}
