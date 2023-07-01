using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;



using Microsoft.AspNetCore.SignalR;

namespace Core;

public class FileRepository : IRoutesRepository
{
  private readonly Dictionary<string, Dictionary<string, uint>> _hubs = new();

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
}
