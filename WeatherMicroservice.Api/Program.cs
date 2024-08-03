using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using WeatherMicroservice.Core.Enums;
using WeatherMicroservice.Core.Interfaces;
using WeatherMicroservice.Infrastructure.Data;
using WeatherMicroservice.Infrastructure.Repositories;
using WeatherMicroservice.Infrastructure.Services;
using Newtonsoft.Json.Converters;

namespace WeatherMicroservice.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                });

            builder.Services.AddHttpClient();
            builder.Services.AddScoped<IWeatherService, WeatherService>();
            builder.Services.AddScoped<IWeatherRepository, WeatherRepository>();
            builder.Services.AddDbContext<WeatherDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Add Swagger/OpenAPI support
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Weather API", Version = "v1" });
                c.UseAllOfToExtendReferenceSchemas();
                c.EnableAnnotations();
                c.MapType<MeasurementType>(() => new OpenApiSchema
                {
                    Type = "string",
                    Enum = Enum.GetNames(typeof(MeasurementType))
                            .Select(name => new OpenApiString(name) as IOpenApiAny)
                            .ToList()
                });
                c.MapType<Station>(() => new OpenApiSchema
                {
                    Type = "string",
                    Enum = Enum.GetNames(typeof(Station))
                            .Select(name => new OpenApiString(name) as IOpenApiAny)
                            .ToList()
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Weather API V1");
                    c.RoutePrefix = string.Empty;
                });
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
