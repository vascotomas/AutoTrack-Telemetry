using System.Threading.Channels;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using AutoTelemetryWorker.Processor;
using AutoTelemetryAPI.Entities.Entities;

namespace AutoTelemetryWorker.Worker
{
    public class JobWorker : BackgroundService
    {
        private readonly Channel<QueueMessage> _channel;
        private readonly IServiceProvider _provider;
        private readonly ILogger<JobWorker> _logger;
        private readonly int _parallelism;

        public JobWorker(Channel<QueueMessage> channel, IServiceProvider provider, ILogger<JobWorker> logger, int parallelism = 4)
        {
            _channel = channel;
            _provider = provider;
            _logger = logger;
            _parallelism = parallelism;
        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            _logger.LogInformation("JobWorker iniciado con paralelismo de {Parallelism}", _parallelism);

            var workers = Enumerable.Range(0, _parallelism)
                .Select(_ => RunWorker(ct));

            await Task.WhenAll(workers);
        }

        private async Task RunWorker(CancellationToken ct)
        {
            await foreach (var msg in _channel.Reader.ReadAllAsync(ct))
            {

                try
                {
                    using var scope = _provider.CreateScope();
                    var processor = scope.ServiceProvider.GetRequiredService<TelemetryProcessor>();
                    await processor.ProcessAsync(msg.Job, ct);
                }
                catch (Exception ex)
                {
                   _logger.LogError(ex, $"Error crítico procesando el chasis {msg.Job?.ChasisId}");
                }
            }
        }
    }
}
