using CheapestTravel;

using System;
using System.Threading.Tasks;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using Microsoft.VisualBasic;

namespace CLI;

internal class Program
{
  private static async Task<int> Main(string[] args)
  {
    var root = new RootCommand
    {
      new Option<string>("--import-file", description: "Import CSV file to Cheap Flight System"),
      new Option<string>("--trip", description: "Import CSV file to Cheap Flight System"),
    };

    var tripCommand = new Command("--trip", description: "Find the cheapest route between two points");
    tripCommand.SetHandler((trip) => Console.WriteLine($"Trip: {trip}"));

    root.Handler = CommandHandler.Create<string, string>((importFile, trip) => {
      Console.WriteLine($"Import: {importFile}");
      Console.WriteLine($"Trip: {trip}");
    });

    return await root.InvokeAsync(args);
  }
}
