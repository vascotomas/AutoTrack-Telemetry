using System;
using System.Collections.Generic;
using System.Text;

namespace AutoTelemetryAPI.Entities.Entities
{
    public class QueueMessage
    {
        public TelemetryEvent Job { get; set; } = null!;
    }
}
