using PortMonitor.Models;
using System.Text.Json;

namespace PortMonitor.Services
{
    public class GeoIpService
    {
        public async Task<IpInfoResponse> LookupAsync(string ip)
        {
            using HttpClient client = new HttpClient();
            string url = $"https://ipinfo.io/{ip}/json";

            string json = await client.GetStringAsync(url);
            return JsonSerializer.Deserialize<IpInfoResponse>(json)!;
        }
    }
}