using Bored_But_Broke_back_end.ExternalApis.Geoapify;
using Bored_But_Broke_back_end.ExternalApis.OpenMeteo;
using Bored_But_Broke_back_end.ExternalApis.Yelp;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Primitives;

namespace Bored_But_Broke_back_end.HealthChecks
{
    public class ExternalApisHealthCheck : IHealthCheck
    {
        private readonly IYelpClient _yelpClient;
        private readonly IGeoapifyClient _geoapifyClient;
        private readonly IOpenMeteoClient _openMeteoClient;
        public ExternalApisHealthCheck(IYelpClient yelpClient, IGeoapifyClient geoapifyClient, IOpenMeteoClient openMeteoClient)
        {
            _yelpClient = yelpClient;
            _geoapifyClient = geoapifyClient;
            _openMeteoClient = openMeteoClient;
        }
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var data = new Dictionary<string, object>();

            try
            {
                var yelpQuery = new Dictionary<string, StringValues>
                {
                    { "location", "M1 1AE" },
                    { "limit", "1" }
                };
                await _yelpClient.BusinessesSearchAsync(yelpQuery, cancellationToken);
                data["Yelp"] = "Healthy";
            }
            catch (Exception ex)
            {
                data["Yelp"] = $"Unhealthy: {ex.Message}";
            }
            try
            {
                await _geoapifyClient.ForwardGeocodingAsync(
                    "M1 1AE",
                    cancellationToken);

                data["Geoapify"] = "Healthy";
            }
            catch (Exception ex)
            {
                data["Geoapify"] = $"Unhealthy: {ex.Message}";
            }
            bool allHealthy = data.Values.All(v => v.ToString() == "Healthy");

            return allHealthy
                ? HealthCheckResult.Healthy(
                    "All external APIs are healthy.",
                    data)
                : HealthCheckResult.Unhealthy(
                    "One or more external APIs are unavailable.",
                    data: data);
        }
    }
}
