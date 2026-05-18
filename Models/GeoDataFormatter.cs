using PortMonitor.Models;

namespace PortMonitor.Services
{
    public static class GeoDataFormatter
    {
        public static IEnumerable<string> Format(IpInfoResponse info)
        {
            yield return $"IP: {info.ip}";
            yield return $"Hostname: {info.hostname}";
            yield return $"City: {info.city}";
            yield return $"Region: {info.region}";
            yield return $"Country: {info.country}";
            yield return $"Location: {info.loc}";
            yield return $"Org: {info.org}";
            yield return $"Postal: {info.postal}";
            yield return $"Timezone: {info.timezone}";
        }
    }
}