using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using CheapestTravel;

using Microsoft.AspNetCore.SignalR;

namespace Core;

public class FileRepository : IRoutesRepository
{
  private readonly Dictionary<string, Dictionary<string, uint>> _hubs = new();
  private readonly string _filePath;
  private static string LINE_PATTERN = @"[A-Za-z]{1,},[A-Za-z]{1,},\d{1,}";


  public FileRepository()
  {

  }

  public FileRepository(string filePath = "store/input-routes.csv")
  {
    _filePath = filePath;
    var lines = RouteFile.ReadFile(filePath);
    AddRoute(lines);
  }

  public void AddRoute(string source, string destination, uint cost)
  {
    if (source.Equals(destination, StringComparison.OrdinalIgnoreCase))
    {
      throw new ArgumentException($"CIRCULAR_REFERENCE > {source}, {destination}");
    }
    AddBidirecionalHub(source, destination, cost);
    AddBidirecionalHub(destination, source, cost);
  }

  public IDictionary<string, uint> Get(string key)
  {
    return _hubs[key]; //Danger
  }

  public IReadOnlyDictionary<string, IReadOnlyDictionary<string, uint>> GetAll() =>
    _hubs.ToDictionary(h => h.Key, h => (IReadOnlyDictionary<string, uint>)h.Value);

  public bool HasHub(string hubName) =>
    _hubs.ContainsKey(hubName);

  private void AddBidirecionalHub(string pointA, string pointB, uint cost)
  {
    if (_hubs.ContainsKey(pointA))
      _hubs[pointA].Add(pointB, cost);
    else
      _hubs[pointA] = new Dictionary<string, uint> { { pointB, cost } };
  }

  public void AddRoute(IEnumerable<string> lines)
  {
    var regex = new Regex(LINE_PATTERN);
    var lineCount = 1;
    foreach (var line in lines)
    {
      if (regex.IsMatch(line))
      {
        var info = line.Split(",");
        var source = info[0].ToUpper();
        var destination = info[1].ToUpper();

        AddRoute(source, destination, Convert.ToUInt32(info[2]));
      }
      else
        throw new ArgumentException($"Line [{lineCount}] input format mismatch 'string, string, uint' > {line}");
      lineCount++;
    }
  }

  public void Save()
  {
    var lines = new List<(string, string, uint)>();
    foreach (var hub in _hubs)
    {
      foreach (var sub in hub.Value)
      {
        (string, string, uint) t = (hub.Key, sub.Key, sub.Value);
        if (!lines.Contains((sub.Key, hub.Key, sub.Value)))
          lines.Add(t);
      }
    }

    RouteFile.WriteFile(_filePath, lines.Select(h => $"{h.Item1},{h.Item2},{h.Item3}").ToArray());
  }
}
