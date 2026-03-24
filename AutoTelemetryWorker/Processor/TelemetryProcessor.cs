using AutoTelemetry.Infrastructure.Context;
using AutoTelemetryAPI.Entities;
using AutoTelemetryAPI.Entities.Entities;
using AutoTelemetryWorker.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using AutoTelemetryEntities.Enums;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

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

            _logger.LogInformation("Iniciando validación del chasis {ChasisId}. TransactionId: {TransactionId}",evento.ChasisId, evento.Id);

            bool yaExiste = _dbContext.TelemetryEvents.Any(x => x.ChasisId == evento.ChasisId);

            if (yaExiste)
            {
                _logger.LogWarning("El chasis {ChasisId} ya existe. Descartando procesamiento. TransactionId: {TransactionId}",evento.ChasisId, evento.Id);

                await _notifier.NotifyStatusChangedAsync(evento.Id, EstadoTelemetria.Error_Duplicado, ct);
                return;
            }

            _logger.LogInformation("Iniciando trabajo en estación para chasis {ChasisId}. TransactionId: {TransactionId}", evento.ChasisId, evento.Id);

            await Task.Delay(2000, ct);

            if (evento.Temperatura > 100)
            {
                evento.Estado = EstadoTelemetria.Alerta_Temperatura;

                _logger.LogWarning("ALERTA - El chasis {ChasisId} superó la temperatura máxima con {Temperatura}°C. TransactionId: {TransactionId}",evento.ChasisId, evento.Temperatura, evento.Id);
            }
            else
            {
                evento.Estado = EstadoTelemetria.Procesado;
            }

            _dbContext.TelemetryEvents.Add(evento);
            await _dbContext.SaveChangesAsync(ct);

            _logger.LogInformation("Chasis {ChasisId} guardado exitosamente con estado: {Estado}. TransactionId: {TransactionId}", evento.ChasisId, evento.Estado, evento.Id);

            await _notifier.NotifyStatusChangedAsync(evento.Id, evento.Estado, ct);
        }
    }
}