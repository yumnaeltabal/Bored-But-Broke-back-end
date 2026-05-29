using Bored_But_Broke_back_end.ExternalAPI.WeatherApi;
using Bored_But_Broke_back_end.Models;
using Bored_But_Broke_back_end.Services;

namespace Bored_But_Broke_back_end
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddHttpClient<IWeatherClient, WeatherClient>();
            builder.Services.AddScoped<IWeatherService, WeatherService>();

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.MapPost("/weather", async (
                WeatherRequest request,
                IWeatherService service) =>
            {
                var result = await service.GetWeatherAndForwardAsync(request);
                return Results.Ok(result);
            });

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
