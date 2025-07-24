var builder = DistributedApplication.CreateBuilder(args);

var mongo = builder.AddMongoDB("mongo");
var mongodb = mongo.AddDatabase("mongodb");

var gunslinger = builder.AddProject<Projects.Treel_Games_GunslingerApi>("treel-games-quantumgunslinger");
var crashgame = builder.AddProject<Projects.Treel_Games_CrashApi>("treel-games-crashapi").WithExternalHttpEndpoints();


builder.AddProject<Projects.VolatilTreels_Api>("volatiltreels-api")
    .WithReference(mongodb)
    .WithReference(gunslinger)
    .WithReference(crashgame)
    .WithExternalHttpEndpoints();

builder.Build().Run();
