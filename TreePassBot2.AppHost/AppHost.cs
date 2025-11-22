var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.BackManager_Server>("backmanager-server");

builder.Build().Run();
