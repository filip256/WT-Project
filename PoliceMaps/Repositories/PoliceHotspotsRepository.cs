using Microsoft.EntityFrameworkCore;
using PoliceMaps.Contexts;
using PoliceMaps.Models.DTOs;
using PoliceMaps.Entities;
using PoliceMaps.Helpers;
using PoliceMaps.Repositories.Generic;
using static PoliceMaps.Constants.Constants;

namespace PoliceMaps.Repositories
{
    public interface IPoliceHotspotsRepository : IGenericRepository<PoliceHotspot>
    {
        Task<PoliceHotspot?> FindByExternalIdAsync(string externalId);
        Task<List<PoliceHotspot>> FindNearAsync(double latitude, double longitude, double radius);
        Task<PoliceHotspot?> FindClosestAsync(double latitude, double longitude, double? maxDistance);
        Task AddOrUpdateAsync(PoliceLocationModel model);
        Task AddOrUpdateAsync(IEnumerable<PoliceLocationModel> models);
        Task<List<HotspotExportModel>> GetAsExportAsync(int? maxEntries);
    }

    public class PoliceHotspotsRepository : GenericRepository<PoliceHotspot, MapsDbContext>, IPoliceHotspotsRepository
    {
        public PoliceHotspotsRepository(MapsDbContext context) : 
            base(context)
        {
        }

        public async Task<PoliceHotspot?> FindByExternalIdAsync(string externalId)
        {
            return await base.GetQuery().FirstOrDefaultAsync(p => p.ExternalId == externalId);
        }

        public async Task<List<PoliceHotspot>> FindNearAsync(double latitude, double longitude, double radius)
        {
            return await base.GetQuery()
                .Where(p => Formulas.SquaredDistance(p.Latitude, p.Longitude, latitude, longitude) <= radius * radius)
                .ToListAsync();
        }

        public async Task<PoliceHotspot?> FindClosestAsync(double latitude, double longitude, double? maxDistance)
        {
            var result = base.GetQuery()
                .AsEnumerable()
                .OrderBy(p => Formulas.SquaredDistance(p.Latitude, p.Longitude, latitude, longitude));

            var first = result.FirstOrDefault();

            if (first == null)
                return null;

            if (maxDistance != null && Formulas.SquaredDistance(first.Latitude, first.Longitude, latitude, longitude) > maxDistance * maxDistance)
                return null;

            return first;
        }

        public async Task AddOrUpdateAsync(PoliceLocationModel model)
        {
            var existing = await this.FindByExternalIdAsync(model.ExternalId);

            if (existing == null)
                existing = await this.FindClosestAsync(model.Latitude, model.Longitude, MaxSpatialEqualityDistance);

            if (existing != null)
            {
                if(DateTime.Now - existing.LastUpdate > MinPlaceReocurranceTime)
                    existing.Severity += 1;

                existing.LastUpdate = DateTime.Now;

                await base.AddOrUpdateAsync(existing);
                return;
            }

            await base.AddAsync(new PoliceHotspot
            {
                ExternalId = model.ExternalId,
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                Severity = PlaceSeverityIncrement,
                LastUpdate = DateTime.Now,
                FirstOcurrence = DateTime.Now,
            });
        }

        public async Task AddOrUpdateAsync(IEnumerable<PoliceLocationModel> models)
        {
            foreach (var m in models)
                await this.AddOrUpdateAsync(m);
        }

        public async Task<List<HotspotExportModel>> GetAsExportAsync(int? maxEntries)
        {
            if (maxEntries != null)
            {
                return await base.GetQuery().OrderByDescending(h => h.LastUpdate)
                    .Take((int)maxEntries).Select(h => new HotspotExportModel(h)).ToListAsync();
            }

            return await base.GetQuery().OrderByDescending(h => h.LastUpdate)
                .Select(h => new HotspotExportModel(h)).ToListAsync();
        }
    }
}
