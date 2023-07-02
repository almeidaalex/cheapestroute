using CheapestTravel;

using System;
using System.Threading.Tasks;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using Core;

namespace CLI;

internal class Program
{
  private static async Task<int> Main(string[] args)
  {
    var root = new RootCommand
    {
      new Option<string>("--source", description: "Import CSV file to Cheap Flight System"),
      new Option<string>("--trip", description: "Trip from point A to point B"),
    };

    root.Handler = CommandHandler.Create<string, string>((source, trip) => {
      Console.WriteLine($"Import: {source}");
      Console.WriteLine($"Trip: {trip}");
      var tripInput = trip.Split(' ');
      var network = new FlightNetwork(new FileRepository(filePath: source));
      var route = network.CheapRoute(tripInput[0], tripInput[1]);
      Console.WriteLine($"The cheapest route is: {route}");
    });

    return await root.InvokeAsync(args);
  }
}
