using System;
using System.Collections.Generic;
using System.Text;
using AutoTelemetryEntities.Enums;

namespace AutoTelemetryEntities.Entities
{
    public class TelemetryEvent
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string ChasisId { get; set; } = string.Empty;
        public string Estacion { get; set; } = string.Empty;
        public double Temperatura { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public  EstadoTelemetria Estado { get; set; } = EstadoTelemetria.Pendiente;
    }
}
