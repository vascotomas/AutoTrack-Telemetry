namespace AutoTelemetryEntities.Exceptions
{
    public sealed class DuplicateChasisException : Exception
    {
        public DuplicateChasisException(string chasisId)
            : base($"El chasis '{chasisId}' ya existe.")
        {
            ChasisId = chasisId;
        }

        public string ChasisId { get; }
    }
}
