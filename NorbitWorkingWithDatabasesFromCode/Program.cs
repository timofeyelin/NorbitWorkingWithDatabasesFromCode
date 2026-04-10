using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NorbitWorkingWithDatabasesFromCode;
using NorbitWorkingWithDatabasesFromCode.Models;
using NorbitWorkingWithDatabasesFromCode.Repositories.AdoNet;
using NorbitWorkingWithDatabasesFromCode.Repositories.EntityFramework;
using NorbitWorkingWithDatabasesFromCode.Repositories.Interfaces;

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddUserSecrets<Program>(true);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException("Connection string is missing.");

var accessTypeRaw = builder.Configuration["AccessType"]
                    ?? throw new InvalidOperationException("AccessType is missing in config.");

if (!Enum.TryParse<ProjectDataAccessType>(accessTypeRaw, true, out var accessType))
    throw new NotSupportedException($"AccessType '{accessTypeRaw}' is not supported.");

switch (accessType)
{
    case ProjectDataAccessType.AdoNet:
        builder.Services.AddTransient<IProjectRepository>(_ => new ProjectAdoRepository(connectionString));
        Console.WriteLine("Запуск в режиме ADO.NET");
        break;
    case ProjectDataAccessType.EntityFramework:
        builder.Services.AddDbContext<EducationDbContext>(options =>
            options.UseSqlServer(connectionString));
        builder.Services.AddTransient<IProjectRepository, ProjectEfRepository>();
        Console.WriteLine("Запуск в режиме Entity Framework");
        break;
    default:
        throw new NotSupportedException($"AccessType '{accessType}' is not supported.");
}

builder.Services.AddTransient<ProjectDemoRunner>();

using var host = builder.Build();
using var scope = host.Services.CreateScope();
var runner = scope.ServiceProvider.GetRequiredService<ProjectDemoRunner>();
await runner.RunAsync();