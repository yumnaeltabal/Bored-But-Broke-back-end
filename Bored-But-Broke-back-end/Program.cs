using Bored_But_Broke_back_end.ExternalApis.Geoapify;
using Bored_But_Broke_back_end.ExternalApis.OpenMeteo;
using Bored_But_Broke_back_end.ExternalApis.Yelp;
using Bored_But_Broke_back_end.Middlewares;
using Bored_But_Broke_back_end.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace Bored_But_Broke_back_end
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddScoped<IPlaceService, PlaceService>();
            builder.Services.AddScoped<ILocationService, LocationService>();
            builder.Services.AddScoped<IWeatherService, WeatherService>();
            builder.Services.AddScoped<IYelpClient, YelpClient>();
            builder.Services.AddScoped<IGeoapifyClient, GeoapifyClient>();

            builder.Services.AddHttpClient<IYelpClient, YelpClient>(client =>
            {
                string _baseURL = "https://api.yelp.com/v3/";
                string _apiKey = builder.Configuration["YELP_API_KEY"]
                    ?? throw new InvalidOperationException("Environment variable 'YELP_API_KEY' is missing.");
                
                client.BaseAddress = new Uri(_baseURL);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
                client.Timeout = TimeSpan.FromSeconds(20);
            });

            builder.Services.AddHttpClient<IGeoapifyClient, GeoapifyClient>(client =>
            {
                string _baseURL = "https://api.geoapify.com/v1/";
                
                client.BaseAddress = new Uri(_baseURL);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.Timeout = TimeSpan.FromSeconds(20);
            });

            builder.Services.AddHttpClient<IOpenMeteoClient, OpenMeteoClient>();

            builder.Services.AddExceptionHandler<ExceptionHandler>();
            builder.Services.AddProblemDetails();

            builder.Services.AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var errors = context.ModelState
                            .Where(e => e.Value?.Errors.Count > 0)
                            .Select(e => $"{e.Key}: {string.Join(", ", e.Value!.Errors.Select(x => x.ErrorMessage))}")
                            .ToList();

                        var problemDetails = new ValidationProblemDetails(context.ModelState)
                        {
                            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                            Detail = string.Join(" | ", errors),
                        };

                        var traceId = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;
                        problemDetails.Extensions["traceId"] = traceId;

                        return new BadRequestObjectResult(problemDetails);
                    };
                });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseExceptionHandler();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();


            app.MapPost("/weather", async (
                WeatherRequest request,
                IWeatherService service) =>
            {
                var isIndoor = await service.GetWeatherAndForwardAsync(request);

                return Results.Ok(isIndoor);
            });

            app.Run();
        }
    }
}
