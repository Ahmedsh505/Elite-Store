using Elite.Extensions;
using Elite.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace Elite
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    // ✅ FIX: Handle circular references in JSON serialization
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                });

            // API Documentation
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddOpenApi();

            // Register Elite services (Database, Repositories, Services)
            builder.Services.AddEliteServices(builder.Configuration);

            // CORS (if needed for frontend)
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowAll");

            // ✅ COMMENTED OUT - Uncomment when JWT auth is implemented
            // app.UseAuthentication();
            // app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}