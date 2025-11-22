var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.TreePassBot2>("treepassbot2");

builder.AddProject<Projects.BackManager_Server>("backmanager-server");

builder.Build().Run();
