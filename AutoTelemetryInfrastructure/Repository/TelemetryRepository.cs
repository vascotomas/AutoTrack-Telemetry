using AutoTelemetryEntities.Entities;
using AutoTelemetryEntities.Interfaces;
using AutoTelemetryInfrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace AutoTelemetryInfrastructure.Repository
{
    public class TelemetryRepository : ITelemetryRepository
    {
        private readonly AppDbContext _dbContext;

        public TelemetryRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> ExisteChasisAsync(string chasisId, CancellationToken ct) => await _dbContext.TelemetryEvents.AnyAsync(x => x.ChasisId == chasisId, ct);

        public async Task GuardarEventoAsync(TelemetryEvent evento, CancellationToken ct)
        {
            _dbContext.TelemetryEvents.Add(evento);
            await _dbContext.SaveChangesAsync(ct);          
        }
    }
}
