using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;

public class Graph
{
    private Dictionary<string, Dictionary<string, int>> _hubs = new Dictionary<string, Dictionary<string, int>>();

    public void AddHub(string source, string destination, int cost)
    {
        AddBidirecionalHub(source, destination, cost);
        AddBidirecionalHub(destination, source, cost);
    }

    private void AddBidirecionalHub(string pointA, string pointB, int cost)
    {
        if (_hubs.ContainsKey(pointA))
            _hubs[pointA].Add(pointB, cost);
        else
            _hubs[pointA] = new Dictionary<string, int> { { pointB, cost } };
    }

    public List<string> shortest_path(string start, string finish)
    {
        var previous = new Dictionary<string, string>();
        var distances = new Dictionary<string, int>();
        var nodes = new List<string>();

        List<string> path = null;

        foreach (var vertex in _hubs)
        {
            if (vertex.Key == start)
            {
                distances[vertex.Key] = 0;
            }
            else
            {
                distances[vertex.Key] = int.MaxValue;
            }

            nodes.Add(vertex.Key);
        }

        while (nodes.Count != 0)
        {
            nodes.Sort((x, y) => distances[x] - distances[y]);

            var smallest = nodes[0];
            nodes.Remove(smallest);

            if (smallest == finish)
            {
                path = new List<string>();
                while (previous.ContainsKey(smallest))
                {
                    path.Add(smallest);
                    smallest = previous[smallest];
                }

                break;
            }

            if (distances[smallest] == int.MaxValue)
            {
                break;
            }

            foreach (var neighbor in _hubs[smallest])
            {
                var alt = distances[smallest] + neighbor.Value;
                if (alt < distances[neighbor.Key])
                {
                    distances[neighbor.Key] = alt;
                    previous[neighbor.Key] = smallest;
                }
            }
        }

        return path;
    }

    public Dictionary<string, Leg> CheapRoute(string from, string to)
    {
        Dictionary<string, Leg> visited = new Dictionary<string, Leg>();
        Dictionary<string, Leg> airports = _hubs.ToDictionary(hub => hub.Key , hub => new Leg { CurrentCost = int.MaxValue });

        var currentVisit = from;
        airports[currentVisit].CurrentCost = 0;
        
        while (airports.Any()) 
        {
            var nextVisit = Visit(currentVisit);
            currentVisit = nextVisit.Key;
        }

        KeyValuePair<string, Leg> Visit(string origin)
        {
            var directHubs = _hubs[origin];
            var originAirport = airports[origin];

            foreach (var directHub in directHubs)
            {
                if (!airports.ContainsKey(directHub.Key)) continue;

                var visit = airports[directHub.Key];
                var newCost = directHub.Value + originAirport.CurrentCost;

                if (visit.CurrentCost == int.MaxValue || (visit.CurrentCost > newCost))
                {
                    visit.CurrentCost = newCost;
                    visit.Previous = origin;
                }
            }
            visited.Add(origin, originAirport);
            airports.Remove(origin);
            return airports.Where(a => a.Value.Previous == origin).OrderBy(a => a.Value.CurrentCost).First();
        }

        return visited;
    }

    public class Leg
    {
        public string Previous;
        public int CurrentCost = int.MaxValue;
    }


    public IEnumerable<string> CheapRoute2(string from, string to)
    {
        var path_weight = _hubs.ToDictionary(hub => hub.Key, hub => int.MaxValue);
        var previous = new Dictionary<string, string>();
        var cheapestTravel = new List<string>();

        path_weight[from] = 0;

        while (path_weight.Any())
        {
            var n = path_weight.OrderBy(r => r.Value).First();

            var hubs = _hubs[n.Key];
            foreach (var hub in hubs)
            {
                if (!path_weight.ContainsKey(hub.Key)) continue;

                var cost = n.Value + hub.Value;
                if (path_weight[hub.Key] == int.MaxValue || cost < path_weight[hub.Key])
                {
                    path_weight[hub.Key] = cost;
                    previous[hub.Key] = n.Key;
                }
            }
            path_weight.Remove(n.Key);

            if (n.Key == to)
            {
                string airport = to;
                while(previous.ContainsKey(airport))
                {
                    cheapestTravel.Add(airport);
                    airport = previous[airport];
                }
                cheapestTravel.Add(from);
                cheapestTravel.Reverse();
                break;
                
            }
        }

        return cheapestTravel;
    }
}


