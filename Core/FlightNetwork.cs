using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Core;

using Microsoft.AspNetCore.SignalR;

namespace CheapestTravel
{
  public class FlightNetwork
  {
    private readonly IRoutesRepository _routesRepository;

    public FlightNetwork(IRoutesRepository routesRepository)
    {
      _routesRepository = routesRepository;
    }

    public CheapestRouteResult CheapRoute(string from, string to)
    {
      from = from.ToUpper();
      to = to.ToUpper();

      var net = _routesRepository.GetAll();
      var travelFare = net.ToDictionary(hub => hub.Key, hub => uint.MaxValue);
      var previous = new Dictionary<string, string>();
      var cheapestTravel = new List<string>();
      KeyValuePair<string, uint> visit = new();

      if (!travelFare.ContainsKey(from))
        return new CheapestRouteResult($"NO_ORIGIN_FOUND > {from}");

      if (!travelFare.ContainsKey(to))
        return new CheapestRouteResult($"NO_DESTINATION_FOUND > {to}");

      travelFare[from] = 0;

      while (travelFare.Any())
      {
        visit = travelFare.OrderBy(r => r.Value).First();

        var hubs = net[visit.Key];
        foreach (var hub in hubs)
        {
          if (!travelFare.ContainsKey(hub.Key)) continue;

          if (visit.Value == uint.MaxValue)
          {
            return new CheapestRouteResult($"NO_ROUTE");
          }

          var cost = visit.Value + hub.Value;
          if (travelFare[hub.Key] == uint.MaxValue || cost < travelFare[hub.Key])
          {
            travelFare[hub.Key] = cost;
            previous[hub.Key] = visit.Key;
          }
        }
        travelFare.Remove(visit.Key);

        if (visit.Key == to)
        {
          string airport = to;
          while (previous.ContainsKey(airport))
          {
            cheapestTravel.Add(airport);
            airport = previous[airport];
          }
          cheapestTravel.Add(from);
          cheapestTravel.Reverse();
          break;

        }
      }

      return new CheapestRouteResult(cheapestTravel, visit.Value);
    }
  }

  public class CheapestRouteResult : Result
  {
    public CheapestRouteResult(string error)
    {
      base.AddError(error);
    }

    public CheapestRouteResult(IEnumerable<string> routes, uint cost)
    {
      Routes = routes;
      Cost = cost;
    }

    public IEnumerable<string> Routes { get; }
    public uint Cost { get; }

    public override string ToString()
    {
      var formatedRoutes = string.Join(" - ", this.Routes);

      return $"{formatedRoutes} > ${this.Cost}";
    }

  }

  public abstract class Result
  {
    private readonly List<string> _errors = new List<string>();

    public void AddError(string error)
    {
      _errors.Add(error);
    }

    public bool HasErrors => _errors.Any();

    public IReadOnlyList<string> Errors => _errors;

  }

}
