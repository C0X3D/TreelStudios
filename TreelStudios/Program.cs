using Cox.SignalRDataModel.Interfaces;
using Cox.SignalRHub;
using DatabaseContext;
using DatabaseContext.Interfaces;
using DatabaseContext.Repositories;
using TreelStudios.SignalR.DataLayer;
using TreelStudios.SignalR.DataLayer.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.WithOrigins("https://gunslinger.coxino.ro") // Specify the exact origin
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .AllowCredentials(); // Allow credentials
        });
});
builder.AddMongoDBClient("mongodb");

//uses mongodb

// Add services to the container.
builder.Services.AddSignalR(config => { config.EnableDetailedErrors = true; });

builder.Services.AddScoped<ISpinLogicRepository, SpinLogicRepository>();
builder.Services.AddScoped<IBalanceWithdrawRepository, BalanceWithdrawRepository>();
builder.Services.AddScoped<IBalanceDepositRepository, BalanceDepositRepository>();
builder.Services.AddScoped<_IDatabaseContextManager, DatabaseContextManager>();
builder.Services.AddSingleton<IClientManagementService, ClientManagementService>();
builder.Services.AddSingleton<IHubsService, HubsService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowSpecificOrigin");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapHub<SpinHub>(nameof(SpinHub));

app.Run();
