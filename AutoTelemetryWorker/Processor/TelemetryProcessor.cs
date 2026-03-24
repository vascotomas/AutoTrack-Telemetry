using Microsoft.Extensions.Logging;
using AutoTelemetryEntities.Enums;
using AutoTelemetryEntities.Entities;
using AutoTelemetryEntities.Exceptions;
using AutoTelemetryEntities.Interfaces;
namespace AutoTelemetryWorker.Processor
{
    public class TelemetryProcessor
    {
        private readonly ITelemetryRepository _repository;
        private readonly ILogger<TelemetryProcessor> _logger;
        private readonly IRealTimeNotifier _notifier;

        public TelemetryProcessor(ITelemetryRepository repository, ILogger<TelemetryProcessor> logger, IRealTimeNotifier notifier)
        {
            _repository = repository;
            _logger = logger;
            _notifier = notifier;
        }

        public async Task ProcessAsync(TelemetryEvent evento, CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("Iniciando validación del chasis {ChasisId}. TransactionId: {TransactionId}", evento.ChasisId, evento.Id);

                bool yaExiste = await _repository.ExisteChasisAsync(evento.ChasisId, ct);

                if (yaExiste)
                {
                    _logger.LogWarning("El chasis {ChasisId} ya existe. Descartando procesamiento. TransactionId: {TransactionId}", evento.ChasisId, evento.Id);

                    await _notifier.BroadcastEventAsync("TelemetryProcessed", evento.Id, EstadoTelemetria.Error_Duplicado.ToString(), ct);
                    return;
                }

                _logger.LogInformation("Iniciando trabajo en estación para chasis {ChasisId}. TransactionId: {TransactionId}", evento.ChasisId, evento.Id);

                await Task.Delay(2000, ct);

                if (evento.Temperatura > 100)
                {
                    evento.Estado = EstadoTelemetria.Alerta_Temperatura;

                    _logger.LogWarning("ALERTA - El chasis {ChasisId} superó la temperatura máxima con {Temperatura}°C. TransactionId: {TransactionId}", evento.ChasisId, evento.Temperatura, evento.Id);
                }
                else
                {
                    evento.Estado = EstadoTelemetria.Procesado;
                }

                await _repository.GuardarEventoAsync(evento, ct);
                _logger.LogInformation("Chasis {ChasisId} guardado exitosamente con estado: {Estado}. TransactionId: {TransactionId}", evento.ChasisId, evento.Estado, evento.Id);
                await _notifier.BroadcastEventAsync("TelemetryProcessed", evento.Id, evento.Estado.ToString(), ct);

            }
            catch (DuplicateChasisException ex)
            {
                _logger.LogError(ex, "Colisión detectada: El chasis {ChasisId} se intentó insertar dos veces al mismo tiempo.", ex.ChasisId);
                await _notifier.BroadcastEventAsync("TelemetryProcessed", evento.Id, EstadoTelemetria.Error_Duplicado.ToString(), ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error crítico  al intentar guardar el chasis {ChasisId}.", evento.ChasisId);
                await _notifier.BroadcastEventAsync("TelemetryProcessed", evento.Id, EstadoTelemetria.Error_Critico.ToString(), ct);
            }
        }
    }
}