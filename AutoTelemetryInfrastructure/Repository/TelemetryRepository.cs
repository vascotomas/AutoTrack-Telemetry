using AutoTelemetryEntities.Entities;
using AutoTelemetryEntities.Exceptions;
using AutoTelemetryEntities.Interfaces;
using AutoTelemetryInfrastructure.Context;
using Microsoft.Data.Sqlite;
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
            try
            {
                _dbContext.TelemetryEvents.Add(evento);
                await _dbContext.SaveChangesAsync(ct);
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqliteException sqliteEx && sqliteEx.SqliteErrorCode == 19)
            {
                throw new DuplicateChasisException(evento.ChasisId);
            }
        }
    }
}
