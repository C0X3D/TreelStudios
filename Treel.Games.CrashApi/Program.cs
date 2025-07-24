using Cox.SignalRDataModel.Interfaces;
using Cox.SignalRHub;
using OpenTelemetry.Resources;
using Treel.Games.CrashApi.GameManager.SignalR;
using Treel.Games.CrashApi.GameManager.SignalR.Interfaces;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.AddServiceDefaults();
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigin2",
                corsPolicy =>  // Changed "builder" to "corsPolicy" to avoid conflicts
                {
                    corsPolicy.WithOrigins("https://gunslinger.coxino.ro",
                                           "https://crash.coxino.ro",
                                           "http://localhost:5513",
                                           "https://localhost:5514",
                                           "https://volatiltreels-api.redpond-f06deadf.polandcentral.azurecontainerapps.io",
                                           "http://localhost:4200") // All allowed origins
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials(); // Allow credentials
                });
        });
        // Add services to the container.
        builder.Services.AddSignalR(config => { config.EnableDetailedErrors = true; });
        builder.Services.AddSingleton<IClientManagementService, ClientManagementService>();
        builder.Services.AddSingleton<ICrashHubService, CrashHubsService>();
        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        app.MapDefaultEndpoints();

        // Configure the HTTP request pipeline.
        //if (app.Environment.IsDevelopment())
        //{
            app.UseSwagger();
            app.UseSwaggerUI();
       
        //}

        app.UseCors("AllowSpecificOrigin2");
        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();
        app.MapHub<CrashHub>(nameof(CrashHub));

        app.Run();
    }
}