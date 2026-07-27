
using BikeIotWebServer.Auth;
using BikeIotWebServer.Infra;
using BikeIotWebServer.mqtt;
using BikeIotWebServer.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

namespace BikeIotWebServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services
                .AddAuthentication(ApiKeyAuthenticationDefaults.AuthenticationScheme)
                .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(ApiKeyAuthenticationDefaults.AuthenticationScheme, _ => { });

            builder.Services.AddAuthorization();

            builder.Services.AddHostedService<MqttSubscriberService>();
            builder.Services.AddScoped<MqttPublisherService>();
            builder.Services.AddScoped<IBikeRepository, BikeRepository>();
            builder.Services.AddScoped<IBikeLockRepository, BikeLockRepository>();
            builder.Services.AddScoped<BikeTelemetryService>();

            // Add EF Core DbContext
            builder.Services.AddDbContext<BikeContext>(options =>
               options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Bike IoT Web Server",
                    Version = "v1"
                });

                options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.ApiKey,
                    Name = ApiKeyAuthenticationDefaults.HeaderName,
                    In = ParameterLocation.Header,
                    Description = "API key needed to access protected endpoints."
                });

            });

            // Add CORS policy
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

             var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<BikeContext>();
                dbContext.Database.Migrate();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger(options =>
                {
                    options.RouteTemplate = "openapi/{documentName}.json";
                });
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/openapi/v1.json", "v1");
                });
            }

            app.UseHttpsRedirection();

            // Enable CORS middleware (must be registered on the pipeline before authorization/controllers)
            app.UseCors("AllowAll");

            app.UseAuthentication();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
