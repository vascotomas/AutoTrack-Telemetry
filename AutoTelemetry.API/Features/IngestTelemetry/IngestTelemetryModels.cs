namespace AutoTelemetryAPI.Features.IngestTelemetry
{
    public class IngestTelemetryRequest
    {
        public Guid Id { get; set; }
        public string ChasisId { get; set; } = string.Empty;
        public string Estacion { get; set; } = string.Empty;
        public double Temperatura { get; set; }
    }

    public class IngestTelemetryResponse
    {
        public string Message { get; set; } = string.Empty;
        public string ChasisId { get; set; } = string.Empty;
    }
}
