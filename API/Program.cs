using System.Security.Cryptography;

using CheapestTravel;

using Core;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace API;

public class Program
{
  public static void Main(string[] args)
  {
    var builder = WebApplication.CreateBuilder(args);
    var app = builder.Build();

    app.MapGet("/api/route/{from}/{destination}", (string from, string destination) =>
    {
      var network = new FlightNetwork(new FileRepository(filePath: "../Core/store/input-routes.csv"));
      var route = network.CheapRoute(from, destination);
      return Results.Ok(route);
    });

    app.MapPost("/api/route", (Route route) =>
    {
      var repo = new FileRepository(filePath: "../Core/store/input-routes.csv");
      repo.AddRoute(route.From, route.Destination, route.Cost);
      repo.Save();
      return Results.Ok(route);
    });

    app.Run();
  }
}
