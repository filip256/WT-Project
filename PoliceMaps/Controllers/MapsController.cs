using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PoliceMaps.Cummunicators;
using PoliceMaps.Entities;
using PoliceMaps.Models.Pagination;
using PoliceMaps.Repositories;
using System.Formats.Asn1;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;

namespace PoliceMaps.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MapsController : ControllerBase
    {
        private readonly IHotspotsRepository _hotspotsRepository;
        private readonly ISurveyAreasRepository _areasRepository;
        private readonly IWazeCommunicator _wazeCommunicator;

        public MapsController(
            IHotspotsRepository hotspotsRepository,
            ISurveyAreasRepository areasRepository,
            IWazeCommunicator wazeCommunicator)
        {
            _hotspotsRepository = hotspotsRepository;
            _areasRepository = areasRepository;
            _wazeCommunicator = wazeCommunicator;
        }

        [HttpGet("spots")]
        public async Task<IActionResult> GetHotspots([FromQuery] PaginationPropertiesModel model)
        {
            var response = await _hotspotsRepository.GetPagedAsync(model);
            return Ok(response);
        }

        [HttpGet("spots/export")]
        public async Task<IActionResult> GetExport([FromQuery] int? maxEntries)
        {
            var entries = await _hotspotsRepository.GetAsExportAsync(maxEntries);

            var memoryStream = new MemoryStream();
            var writer = new StreamWriter(memoryStream);
            var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture));

            csv.WriteRecords(entries);
            writer.Flush();
            memoryStream.Seek(0, SeekOrigin.Begin);

            string csvContent;
            using (var reader = new StreamReader(memoryStream))
            {
                csvContent = reader.ReadToEnd();
            }
            return Ok(csvContent);
        }

        [HttpGet("spots/near")]
        public async Task<IActionResult> GetHotspotsNear([FromQuery] double latitude, [FromQuery] double longitude, [FromQuery] double radius = 1)
        {
            var response = await _hotspotsRepository.FindNearAsync(latitude, longitude, radius);
            return Ok(response);
        }

        [HttpGet("spots/inside")]
        public async Task<IActionResult> GetHotspotsNear(
            [FromQuery] double startLatitude, [FromQuery] double startLongitude,
            [FromQuery] double endLatitude, [FromQuery] double endLongitude)
        {
            var response = await _hotspotsRepository.FindInsideAsync(startLatitude, startLongitude, endLatitude, endLongitude);
            return Ok(response);
        }

        [HttpDelete("spots/clear")]
        public async Task<IActionResult> DeleteSpots([FromForm] DateTime beforeDate)
        {
            await _hotspotsRepository.DeleteAsync(s => s.LastUpdate < beforeDate);
            return Ok();
        }

        [HttpGet("areas")]
        public async Task<IActionResult> GetAreas([FromQuery] PaginationPropertiesModel model)
        {
            var response = await _areasRepository.GetPagedAsync(model);
            return Ok(response);
        }

        [HttpPost("areas")]
        public async Task<IActionResult> CreateArea([FromBody] SurveyArea area)
        {
            await _areasRepository.AddAsync(area);

            var locations = await _wazeCommunicator.GetLocationsAsync(area.StartLongitude, area.StartLatitude, area.EndLongitude, area.EndLatitude, area.SurveyTypes);
            await _hotspotsRepository.AddOrUpdateAsync(locations);

            return Ok();
        }

        [HttpDelete("areas")]
        public async Task<IActionResult> DeleteArea([FromQuery] int id)
        {
            if (await _areasRepository.GetAsync(id) == null)
                return BadRequest("Area not found.");

            await _areasRepository.DeleteAsync(id);
            return Ok();
        }
    }
}
