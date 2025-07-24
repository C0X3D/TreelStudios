using Treel.Games.DataLayer.GameLogic;
using Treel.Games.DataLayer.Mathematics;
using Treel.Games.DataLayer;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.

builder.Services.AddSingleton<SpinGenerator>();
builder.Services.AddSingleton<GameBaseGameSymbols>();
builder.Services.AddSingleton<GameBonusRoundSymbols>();
builder.Services.AddSingleton<PayTable>();
builder.Services.AddSingleton<PayoutCalculations>();
builder.Services.AddSingleton<BaseGameLogic>();
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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
