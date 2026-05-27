
using Bored_But_Broke_back_end.ExternalApis.Yelp;
using Bored_But_Broke_back_end.Services;
using System.Net.Http.Headers;

namespace Bored_But_Broke_back_end
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddScoped<IPlaceService, PlaceService>();
            builder.Services.AddScoped<IYelpClient, YelpClient>();
           
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

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

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
