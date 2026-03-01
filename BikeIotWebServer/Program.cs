
using BikeIotWebServer.Infra;
using BikeIotWebServer.mqtt;
using Microsoft.EntityFrameworkCore;

namespace BikeIotWebServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddHostedService<MqttService>();
            builder.Services.AddScoped<IBikeRepository, BikeRepository>();

            // Add EF Core DbContext (in-memory for now)
            builder.Services.AddDbContext<BikeContext>(options =>
               options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

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

            



            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/openapi/v1.json", "v1");
                });
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            // Enable CORS middleware (must be registered on the pipeline before authorization/controllers)
            app.UseCors("AllowAll");

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
