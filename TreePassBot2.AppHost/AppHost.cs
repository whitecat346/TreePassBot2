var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.TreePassBot2>("treepassbot2");

builder.Build().Run();
