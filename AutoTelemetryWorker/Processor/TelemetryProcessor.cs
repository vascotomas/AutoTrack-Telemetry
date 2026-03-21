using System;
using System.Collections.Generic;
using System.Text;
using AutoTelemetry.Infrastructure.Context;
using AutoTelemetryAPI.Entities;
using AutoTelemetryAPI.Entities.Entities;
using Microsoft.Extensions.Logging;

namespace AutoTelemetryWorker.Processor
{
    public class TelemetryProcessor
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<TelemetryProcessor> _logger;

        public TelemetryProcessor(AppDbContext dbContext, ILogger<TelemetryProcessor> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task ProcessAsync(TelemetryEvent evento, CancellationToken ct)
        {

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

        }
    }
}
