using PoliceMaps.Cummunicators;
using PoliceMaps.Repositories;
using System.Threading;
using static PoliceMaps.Constants.Constants;

namespace PoliceMaps.Services
{
    public class WazeScraperService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly Random _random;

        public WazeScraperService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            _random = new Random();
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var _surveyAreasRepository = scope.ServiceProvider.GetRequiredService<ISurveyAreasRepository>();
         
                    var areas = await _surveyAreasRepository.GetAllAsync();
                    foreach (var area in areas)
                    {
                        var _wazeCommunicator = scope.ServiceProvider.GetRequiredService<IWazeCommunicator>();
                        var _hotspotsRepository = scope.ServiceProvider.GetRequiredService<IHotspotsRepository>();

                        var locations = await _wazeCommunicator.GetLocationsAsync(area.StartLongitude, area.StartLatitude, area.EndLongitude, area.EndLatitude, area.SurveyTypes);

                        try
                        {
                            await _hotspotsRepository.AddOrUpdateAsync(locations);
                        }
                        catch(Exception e)
                        {
                            Console.WriteLine(e.ToString());
                        }
         
                        await Task.Delay(TimeSpan.FromSeconds(_random.Next(5)), cancellationToken);
                    }
                }
                await Task.Delay(TimeSpan.FromMinutes(WazeRefreshRateBaseMins + _random.Next(WazeRefreshRateMaxRandomMins + 1)), cancellationToken);
            }
        
        }
    }
}
