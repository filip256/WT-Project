using Newtonsoft.Json.Linq;
using PoliceMaps.Cummunicators.Generic;
using PoliceMaps.Models.DTOs;
using PoliceMaps.Models.Enums;
using RestSharp;
using static PoliceMaps.Constants.Constants;

namespace PoliceMaps.Cummunicators
{
    public interface IWazeCommunicator
    {
        Task<List<LocationModel>> GetLocationsAsync(double startLongitude, double startLatitude, double endLongitude, double endLatitude, SpotType allowedTypes);
    }

    public class WazeCommunicator : HttpCommunicator, IWazeCommunicator
    {
        public WazeCommunicator() :
            base("https://www.waze.com")
        {}

        public async Task<List<LocationModel>> GetLocationsAsync(double startLongitude, double startLatitude, double endLongitude, double endLatitude, SpotType allowedTypes)
        {
            var queryParams = new Dictionary<string, string>()
            {
                { "left", startLongitude.ToString() },
                { "top", startLatitude.ToString() },
                { "right", endLongitude.ToString() },
                { "bottom", endLatitude.ToString() },
                { "ma", 800.ToString() },
                { "types", "alerts" }
            };

            var headers = new Dictionary<string, string>()
            {
                { "Accept", "*/*" },
                { "Accept-Encoding", "gzip, deflate, br"},
                { "Connection", "keep-alive" }
            };

            var response = await base.ExecuteRequest(Method.Get, "/row-rtserver/web/TGeoRSS", null, null, queryParams, headers);

            if (response == null)
            {
                Console.WriteLine("Communicator error:\n   Waze request failed.");
                return new List<LocationModel>();
            }

            if(response.Content == null)
            {
                Console.WriteLine("Communicator error:\n   Waze request retured no content with status: " + response.StatusCode);
                return new List<LocationModel>();
            }

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine("Communicator error:\n   Waze request returned with status: " + response.StatusCode + " and content: \"" + response.Content + "\"");
                return new List<LocationModel>();
            }


            var jResponse = JObject.Parse(response.Content);
            var alerts = (JArray)jResponse["alerts"];
            if (alerts == null)
                return new List<LocationModel>();

            var result = new List<LocationModel>();
            foreach (var a in alerts)
            {
                var type = (string)a["type"];
                SpotType typeEnum;
                if (!Enum.TryParse(type, out typeEnum))
                    continue;

                if (!allowedTypes.HasFlag(typeEnum))
                    continue;

                var confidence = (int)a["confidence"] * PlaceConfidenceLevelMultiplier + (a["nThumbsUp"] != null ? (int)a["nThumbsUp"] : 0) * PlaceConfidenceLikeMultiplier;
                if (confidence < MinPlaceConfidence)
                    continue;

                string id = ((string)a["wazeData"]);
                result.Add(new LocationModel
                {
                    ExternalId = id.Substring(id.LastIndexOf(',') + 1),
                    Latitude = (double)a["location"]["y"],
                    Longitude = (double)a["location"]["x"],
                    Type = type,
                    Confidence = confidence,
                    ArrivalTime = DateTime.UtcNow
                });
            }

            return result;
        }
    }
}
