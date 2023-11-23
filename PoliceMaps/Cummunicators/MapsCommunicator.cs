using PoliceMaps.Cummunicators.Generic;
using RestSharp;

namespace PoliceMaps.Cummunicators
{
    public interface IMapsCommunicator
    {
        Task UploadCSVToMap(string csv);
    }

    public class MapsCommunicator : HttpCommunicator, IMapsCommunicator
    {
        public MapsCommunicator() :
            base("https://docs.google.com")
        { }

        public async Task UploadCSVToMap(string csv)
        {
            var queryParams = new Dictionary<string, string>()
            {
                { "authuser", "0" },
            };

            var headers = new Dictionary<string, string>()
            {
                { "Accept", "*/*" },
                { "Accept-Encoding", "gzip, deflate, br"},
                { "Connection", "keep-alive" },
                { "X-Goog-Upload-Header-Content-Length", "87438" },
                { "X-Goog-Upload-Header-Content-Type", "application/vnd.ms-excel" },
                { "X-Goog-Upload-Protocol", "resumable" },
                { "X-Goog-Upload-Command", "start" },
                { "Content-Type", "application/x-www-form-urlencoded;charset=utf-8" },
                { "Cookie", "NID=511=g9min2DfUWydNTMlaoAY9MnxmEAuDkCFbke54Ybkgcg6gbkvQB6SLE1aiA27v3604mQGPVwkBFbJMyX6I73am6Lnymyjn7fsw-JTK7LXjCydWHzNJ33sm57QEIgfD5HGItrQPHnh_yrdskKwoyE9j5GUGB6PGR5jTyQAePCoej-LgJttVpkvTaAPFt7T0O3TKo7Vxma1r3YgB7VjvgPqkQ; __Secure-ENID=15.SE=GdT4aUUhx-J8K87HuLuYQRquxYwi2xhuNrV26dfjtDnhHQI79ocDwAnugrry_9CEcKDMcswbdla-LBu4Tg0WU_HwEN3RPO5sQiwoBvrXM7KFzzUFrOV6J7uARRuM3Sv0jkKwYwSRkMs_J8SXrYztZpGXuEVDEg4x8mrARcQtTHI; CONSENT=PENDING+427; SID=cgi0V25L3cpe6nKPr0h2UbbUpM9OMPGRv58OTGbYdHsuSfBXZwEg8WF-Pkd3Oyz42kDySQ.; __Secure-1PSID=cgi0V25L3cpe6nKPr0h2UbbUpM9OMPGRv58OTGbYdHsuSfBXSgvffi-a4qFKvivhjLYwog.; __Secure-3PSID=cgi0V25L3cpe6nKPr0h2UbbUpM9OMPGRv58OTGbYdHsuSfBXYaPo9rBWZML5D9-5ncbHMQ.; HSID=ACz9BnNajk9jOE6vW; SSID=A-kvfgzFlj-DVx926; APISID=e9q96rKrJrRfT1lH/Aa_fxRS8lnMv-QXUE; SAPISID=EqtQ44GpVQBz_H3a/A-toGijzh5NQStRCm; __Secure-1PAPISID=EqtQ44GpVQBz_H3a/A-toGijzh5NQStRCm; __Secure-3PAPISID=EqtQ44GpVQBz_H3a/A-toGijzh5NQStRCm; SIDCC=ACA-OxOxi1Z9bXnFzvofU34Caoh3s6S-I8b_VYT8CSKZYNC-JicA7PcOgT99aF4galLZqGLqiQ; __Secure-1PSIDCC=ACA-OxPr4i1MnjGgCL8UJK6dZ4S1rNMshszM6htsbnQy6PkFFwYPaF-jHTYjtDA4mypmurzlAg; __Secure-3PSIDCC=ACA-OxO9O_K1-8e9vMYzd3p5RXVjgBbzHFtq6RB4-Vxcwf-C1b3LYkZIB6zjgwnIzey04509" },
            };

            var body = "{\"protocolVersion\":\"0.8\",\"createSessionRequest\":{\"fields\":[{\"external\":{\"name\":\"file\",\"filename\":\"test.csv\",\"put\":{},\"size\":87438}},{\"inlined\":{\"name\":\"title\",\"content\":\"test.csv\",\"contentType\":\"text/plain\"}},{\"inlined\":{\"name\":\"addtime\",\"content\":\"1698844379992\",\"contentType\":\"text/plain\"}},{\"inlined\":{\"name\":\"onepick_version\",\"content\":\"v2\",\"contentType\":\"text/plain\"}},{\"inlined\":{\"name\":\"onepick_host_id\",\"content\":\"20\",\"contentType\":\"text/plain\"}},{\"inlined\":{\"name\":\"onepick_host_usecase\",\"content\":\"MapsPro\",\"contentType\":\"text/plain\"}},{\"inlined\":{\"name\":\"isReimportAndMerge\",\"content\":\"1\",\"contentType\":\"text/plain\"}},{\"inlined\":{\"name\":\"My-Maps-Upload-Origin\",\"content\":\"scotty\",\"contentType\":\"text/plain\"}},{\"inlined\":{\"name\":\"My-Maps-Upload-Type\",\"content\":\"text-layer\",\"contentType\":\"text/plain\"}}]}}";

            var response = await base.ExecuteRequest(Method.Get, "/upload/mapspro/resumable", null, body, queryParams, headers);

            if(response == null)
            {
                Console.WriteLine("Communicator error:\n   Map upload request failed.");
                return;
            }
        }
    }
}
