using CodingChallenges.RailRoadService.Core;
using CodingChallenges.RailRoadService.Demo;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddTransient<IRouteProvider, RouteProvider>();
    })
    .Build();

RouteProviderDemo.Run(host);
