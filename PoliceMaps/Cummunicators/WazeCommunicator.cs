using Newtonsoft.Json.Linq;
using PoliceMaps.Cummunicators.Generic;
using PoliceMaps.Models.DTOs;
using RestSharp;
using static PoliceMaps.Constants.Constants;

namespace PoliceMaps.Cummunicators
{
    public interface IWazeCommunicator
    {
        Task<List<PoliceLocationModel>> GetPoliceLocationsAsync(double startLongitude, double startLatitude, double endLongitude, double endLatitude);
    }

    public class WazeCommunicator : HttpCommunicator, IWazeCommunicator
    {
        public WazeCommunicator() :
            base("https://www.waze.com")
        {}

        public async Task<List<PoliceLocationModel>> GetPoliceLocationsAsync(double startLongitude, double startLatitude, double endLongitude, double endLatitude)
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
                return new List<PoliceLocationModel>();
            }

            if(response.Content == null)
            {
                Console.WriteLine("Communicator error:\n   Waze request retured no content with status: " + response.StatusCode);
                return new List<PoliceLocationModel>();
            }

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine("Communicator error:\n   Waze request returned with status: " + response.StatusCode + " and content: \"" + response.Content + "\"");
                return new List<PoliceLocationModel>();
            }


            var jResponse = JObject.Parse(response.Content);
            var alerts = (JArray)jResponse["alerts"];
            if (alerts == null)
                return new List<PoliceLocationModel>();

            var result = new List<PoliceLocationModel>();
            foreach (var a in alerts)
            {
                if ((string)a["type"] != "POLICE")
                    continue;

                var confidence = (int)a["confidence"] * PlaceConfidenceLevelMultiplier + (a["nThumbsUp"] != null ? (int)a["nThumbsUp"] : 0) * PlaceConfidenceLikeMultiplier;
                if (confidence < MinPlaceConfidence)
                    continue;

                string id = ((string)a["wazeData"]);
                result.Add(new PoliceLocationModel
                {
                    ExternalId = id.Substring(id.LastIndexOf(',') + 1),
                    Latitude = (double)a["location"]["y"],
                    Longitude = (double)a["location"]["x"],
                    Type = (string)a["subtype"],
                    Confidence = confidence,
                    ArrivalTime = DateTime.Now
                });
            }

            return result;
        }
    }
}
