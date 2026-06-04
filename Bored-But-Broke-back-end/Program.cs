using Bored_But_Broke_back_end.Data;
using Bored_But_Broke_back_end.ExternalApis.Geoapify;
using Bored_But_Broke_back_end.ExternalApis.OpenMeteo;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Bored_But_Broke_back_end.ExternalApis.Yelp;
using Bored_But_Broke_back_end.HealthChecks;
using Bored_But_Broke_back_end.Middlewares;
using Bored_But_Broke_back_end.Models;
using Bored_But_Broke_back_end.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Bored_But_Broke_back_end.Repositories;

namespace Bored_But_Broke_back_end
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddScoped<IPlaceRepository, PlaceRepository>();
            builder.Services.AddScoped<IFavouriteRepository, FavouriteRepository>();
            builder.Services.AddScoped<IPlaceService, PlaceService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IFavouriteService, FavouriteService>();
            builder.Services.AddScoped<ILocationService, LocationService>();
            builder.Services.AddScoped<IWeatherService, WeatherService>();
            builder.Services.AddScoped<IYelpClient, YelpClient>();
            builder.Services.AddScoped<IGeoapifyClient, GeoapifyClient>();

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=BoredButBroke;Trusted_Connection=True;TrustServerCertificate=True;")
            );

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

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

            builder.Services.AddHealthChecks().AddCheck<ExternalApisHealthCheck>("External API Health Check");
            builder.Services.AddExceptionHandler<ExceptionHandler>();
            builder.Services.AddProblemDetails();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.ExpireTimeSpan = TimeSpan.FromDays(7);
                options.SlidingExpiration = true;
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
                options.Events.OnRedirectToAccessDenied = context =>
                {
                    context.Response.StatusCode = 403;
                    return Task.CompletedTask;
                };
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("BBBFrontEnd", policy =>
                policy.WithOrigins("https://localhost:7256")
                .AllowCredentials()
                .AllowAnyHeader()
                .AllowAnyMethod());
            });

            builder.Services.AddOutputCache(options =>
            {
                options.AddPolicy("PlacesPolicy", policy => policy
                    .Expire(TimeSpan.FromMinutes(10))
                    .SetVaryByQuery("location", "radius", "date", 
                    "startTime", "endTime", "categories", 
                    "ageRange", "budget", "limit")
                );
                options.SizeLimit = 100 * 1024 * 1024;
                options.MaximumBodySize = 2 * 1024 * 1024;
                options.AddBasePolicy(policy => policy.Expire(TimeSpan.FromMinutes(10)));
            });

            builder.Services.AddRateLimiter(options =>
            {
                options.AddPolicy("fixed", httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 20,
                            Window = TimeSpan.FromMinutes(5),
                            QueueLimit = 2,
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                        }));

                options.OnRejected = async (context, ct) =>
                {
                    context.HttpContext.Response.StatusCode = 429;
                    if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                        context.HttpContext.Response.Headers.RetryAfter = ((int)retryAfter.TotalSeconds).ToString();

                    await context.HttpContext.Response.WriteAsync("Too many requests. Try again later.", ct);
                };
            });

            builder.Services.AddAuthorization();

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

            app.UseRouting();

            app.UseOutputCache();
            app.UseRateLimiter();

            app.UseExceptionHandler();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("BBBFrontEnd");
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.MapHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = "application/json";
                    var response = new
                    {
                        status = report.Status.ToString(),
                        checks = report.Entries.Select(e => new
                        {
                            name = e.Key,
                            status = e.Value.Status.ToString(),
                            description = e.Value.Description,
                            data = e.Value.Data
                        })
                    };
                    await context.Response.WriteAsync(
                        JsonSerializer.Serialize(response));
                }
            });

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
