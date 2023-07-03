using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using CheapestTravel;

using Core;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace API;

public class Program
{
  public static void Main(string[] args)
  {
    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddSingleton<IRoutesRepository>(sp => new FileRepository(filePath: "../Core/store/input-routes.csv"));
    var app = builder.Build();

    app.MapGet("/api/route/{from}/{destination}", (string from, string destination) =>
    {
      var repo = app.Services.GetService<IRoutesRepository>();
      var network = new FlightNetwork(repo);
      var route = network.CheapRoute(from, destination);
      return Results.Ok(route);
    });

    app.MapPost("/api/route", (Route route) =>
    {
      var repo = app.Services.GetService<IRoutesRepository>();
      repo.AddRoute(route.From, route.Destination, route.Cost);
      repo.Save();
      return Results.Ok(route);
    });

    app.Run();
  }
}
