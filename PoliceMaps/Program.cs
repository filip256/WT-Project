using Microsoft.EntityFrameworkCore;
using PoliceMaps.Contexts;
using PoliceMaps.Cummunicators;
using PoliceMaps.Extensions;
using PoliceMaps.Repositories;
using PoliceMaps.Services;

namespace PoliceMaps
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<MapsDbContext>(options =>
                options.UseSqlServer(Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")), ServiceLifetime.Transient);

            builder.Services.AddTransient<IWazeCommunicator, WazeCommunicator>();
            builder.Services.AddTransient<IHotspotsRepository, HotspotsRepository>();
            builder.Services.AddTransient<ISurveyAreasRepository, SurveyAreasRepository>();
            builder.Services.AddHostedService<WazeScraperService>();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();



            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.Services.RunMigrations<MapsDbContext>();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}