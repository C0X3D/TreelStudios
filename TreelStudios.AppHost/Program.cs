
var builder = DistributedApplication.CreateBuilder(args);

//var mongo = builder.AddMongoDB("mongo");
//var mongodb = mongo.AddDatabase("mongodb");


var gunslinger = builder.AddProject<Projects.Treel_Games_GunslingerApi>("treel-games-gunslingerapi").WithExternalHttpEndpoints();

//var gunslinger = builder.AddProject<Projects.VolatilTreels_Games_Gunsilnger>("volatiltreels-games-gunsilnger");
//builder.AddProject<Projects.VolatilTreels_Api>("volatiltreels-api").WithExternalHttpEndpoints().WithReference(mongodb).WithReference(gunslinger);

//builder.AddProject<Projects.VolatilTreels_Dashboard>("volatiltreels-dashboard").WithExternalHttpEndpoints();
//builder.AddProject<Projects.VolatilTreels_Api>("volatiltreels-api").WithExternalHttpEndpoints().WithReference(mongodb).WithReference(gunslinger);

//builder.AddProject<Projects.VolatilTreels_Api>("volatiltreels-api").WithExternalHttpEndpoints().WithReference(mongodb).WithReference(gunslinger);

builder.Build().Run();
