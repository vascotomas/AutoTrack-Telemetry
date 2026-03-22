using AutoTelemetry.Infrastructure.Context;
using AutoTelemetryAPI.Entities;
using AutoTelemetryAPI.Entities.Entities;
using AutoTelemetryWorker.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoTelemetryWorker.Processor
{
    public class TelemetryProcessor
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<TelemetryProcessor> _logger;
        private readonly ITelemetryNotifier _notifier;

        public TelemetryProcessor(AppDbContext dbContext, ILogger<TelemetryProcessor> logger, ITelemetryNotifier notifier)
        {
            _dbContext = dbContext;
            _logger = logger;
            _notifier = notifier;
        }

        public async Task ProcessAsync(TelemetryEvent evento, CancellationToken ct)
        {
            _logger.LogInformation($"Iniciando procesamiento del chasis {evento.ChasisId}");
            bool yaExiste = _dbContext.TelemetryEvents.Any(x => x.ChasisId == evento.ChasisId);
            if (yaExiste)
            {
                _logger.LogWarning("El chasis {ChasisId} ya existe. Descartando procesamiento.", evento.ChasisId);
                await _notifier.NotifyStatusChangedAsync(evento.ChasisId,"Error: Chasis Duplicado",ct);
                return;
            } 
                               
            _logger.LogInformation($"Iniciando procesamiento del chasis{evento.ChasisId}");

            await Task.Delay(2000, ct);

            if (evento.Temperatura > 100)
            {
                evento.Estado = "Alerta_Temperatura";
                _logger.LogWarning($"ALERTA - El chasis {evento.ChasisId} superó la temperatura máxima con {evento.Temperatura}°C.");
            }
            else
            {
                evento.Estado = "Procesado";
            }

            _dbContext.TelemetryEvents.Add(evento);
            await _dbContext.SaveChangesAsync(ct);

            _logger.LogInformation($"Chasis {evento.ChasisId} guardado exitosamente con estado: {evento.Estado}");
            await _notifier.NotifyStatusChangedAsync(evento.ChasisId, evento.Estado, ct);
        }
    }
}
