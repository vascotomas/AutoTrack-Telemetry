using System;
using System.Collections.Generic;
using System.Text;

namespace AutoTelemetryEntities.Entities
{
    public class QueueMessage
    {
        public TelemetryEvent Job { get; set; } = null!;
    }
}
