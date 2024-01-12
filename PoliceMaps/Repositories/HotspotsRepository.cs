using Microsoft.EntityFrameworkCore;
using PoliceMaps.Contexts;
using PoliceMaps.Models.DTOs;
using PoliceMaps.Entities;
using PoliceMaps.Helpers;
using PoliceMaps.Repositories.Generic;
using static PoliceMaps.Constants.Constants;
using Microsoft.AspNetCore.Mvc;

namespace PoliceMaps.Repositories
{
    public interface IHotspotsRepository : IGenericRepository<Hotspot>
    {
        Task<Hotspot?> FindByExternalIdAsync(string externalId);
        Task<List<Hotspot>> FindNearAsync(double latitude, double longitude, double radius);
        Task<List<Hotspot>> FindInsideAsync(double startLatitude, double startLongitude, double endLatitude, double endLongitude);
        Task<Hotspot?> FindClosestAsync(double latitude, double longitude, double? maxDistance);
        Task AddOrUpdateAsync(LocationModel model);
        Task AddOrUpdateAsync(IEnumerable<LocationModel> models);
        Task<List<HotspotExportModel>> GetAsExportAsync(int? maxEntries);
    }

    public class HotspotsRepository : GenericRepository<Hotspot, MapsDbContext>, IHotspotsRepository
    {
        public HotspotsRepository(MapsDbContext context) : 
            base(context)
        {
        }

        public async Task<Hotspot?> FindByExternalIdAsync(string externalId)
        {
            return await base.GetQuery().FirstOrDefaultAsync(p => p.ExternalId == externalId);
        }

        public async Task<List<Hotspot>> FindNearAsync(double latitude, double longitude, double radius)
        {
            return base.GetQuery()
                .AsEnumerable()
                .Where(p => Formulas.SquaredDistance(p.Latitude, p.Longitude, latitude, longitude) <= radius * radius)
                .ToList();
        }

        public async Task<List<Hotspot>> FindInsideAsync(double startLatitude, double startLongitude, double endLatitude, double endLongitude)
        {
            var sLat = Math.Min(startLatitude, endLatitude);
            var eLat = Math.Max(startLatitude, endLatitude);
            var sLong = Math.Min(startLongitude, endLongitude);
            var eLong = Math.Max(startLongitude, endLongitude);

            return base.GetQuery()
               .AsEnumerable()
               .Where(p => p.Latitude >= sLat && p.Latitude <= eLat && p.Longitude >= sLong && p.Longitude <= eLong)
               .ToList();
        }

        public async Task<Hotspot?> FindClosestAsync(double latitude, double longitude, double? maxDistance)
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

        public async Task AddOrUpdateAsync(LocationModel model)
        {
            var existing = await this.FindByExternalIdAsync(model.ExternalId);

            if (existing == null)
                existing = await this.FindClosestAsync(model.Latitude, model.Longitude, MaxSpatialEqualityDistance);

            if (existing != null && existing.Type == model.Type)
            {
                if(DateTime.UtcNow - existing.LastUpdate > MinPlaceReocurranceTime)
                    existing.Severity += 1;

                existing.LastUpdate = DateTime.UtcNow;

                await base.AddOrUpdateAsync(existing);
                return;
            }

            await base.AddAsync(new Hotspot
            {
                ExternalId = model.ExternalId,
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                Severity = PlaceSeverityIncrement,
                LastUpdate = DateTime.UtcNow,
                FirstOcurrence = DateTime.UtcNow,
                Type = model.Type
            });
        }

        public async Task AddOrUpdateAsync(IEnumerable<LocationModel> models)
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
