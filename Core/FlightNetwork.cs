using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CheapestTravel
{
    public class FlightNetwork
    {
        private const string LINE_PATTERN = @"[A-Za-z]{1,},[A-Za-z]{1,},\d{1,}";

        private Dictionary<string, Dictionary<string, uint>> _hubs = new Dictionary<string, Dictionary<string, uint>>();

        private void AddHub(string source, string destination, uint cost)
        {
            AddBidirecionalHub(source, destination, cost);
            AddBidirecionalHub(destination, source, cost);
        }

        private void AddBidirecionalHub(string pointA, string pointB, uint cost)
        {
            if (_hubs.ContainsKey(pointA))
                _hubs[pointA].Add(pointB, cost);
            else
                _hubs[pointA] = new Dictionary<string, uint> { { pointB, cost } };
        }

        public CheapestRouteResult CheapRoute(string from, string to)
        {
            var path_weight = _hubs.ToDictionary(hub => hub.Key, hub => uint.MaxValue);
            var previous = new Dictionary<string, string>();
            var cheapestTravel = new List<string>();
            KeyValuePair<string, uint> visit = new KeyValuePair<string, uint>();

            if (!path_weight.ContainsKey(from))
                return new CheapestRouteResult($"NO_ORIGIN_FOUND > {from}");

            if (!path_weight.ContainsKey(to))
                return new CheapestRouteResult($"NO_DESTINATION_FOUND > {to}");

            path_weight[from] = 0;

            while (path_weight.Any())
            {
                visit = path_weight.OrderBy(r => r.Value).First();

                var hubs = _hubs[visit.Key];
                foreach (var hub in hubs)
                {
                    if (!path_weight.ContainsKey(hub.Key)) continue;

                    if (visit.Value == uint.MaxValue)
                    {
                        return new CheapestRouteResult($"NO_ROUTE");
                    }   
                    
                    var cost = visit.Value + hub.Value;
                    if (path_weight[hub.Key] == uint.MaxValue || cost < path_weight[hub.Key])
                    {
                        path_weight[hub.Key] = cost;
                        previous[hub.Key] = visit.Key;
                    }
                }
                path_weight.Remove(visit.Key);

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

        public LoadResult LoadFrom(IEnumerable<string> lines)
        {
            var regex = new Regex(LINE_PATTERN);
            var result = new LoadResult();
            foreach (var line in lines)
            {
                if (regex.IsMatch(line))
                {
                    var info = line.Split(",");
                    AddHub(info[0].ToUpper(), info[1].ToUpper(), Convert.ToUInt32(info[2]));
                }
                else
                    result.AddError($"Linha fora o padrão 'string, string, uint' > {line}");
            }
            return result;            
        }

        public IReadOnlyDictionary<string, IReadOnlyDictionary<string, uint>> Hubs => _hubs.ToDictionary(h => h.Key, h => (IReadOnlyDictionary<string, uint>)h.Value);
    }

    public class CheapestRouteResult
    {   
        private readonly ICollection<string> _errors = new List<string>();

        public CheapestRouteResult(string error)
        {
            _errors.Add(error);
        }

        public CheapestRouteResult(IEnumerable<string> routes, uint cost)
        {
            Routes = routes;
            Cost = cost;
        }

        public IEnumerable<string> Routes { get; }
        public uint Cost { get; }

        public void AddError(string error)
        {
            _errors.Add(error);
        }

        public override string ToString()
        {
            var formatedRoutes = string.Join(" - ", this.Routes);

            return $"{formatedRoutes} > ${this.Cost}";
        }

        public IEnumerable<string> Errors => _errors;

        public bool HasErrors => this.Errors.Any();
    }

    public class LoadResult
    {
        private readonly List<string> _errors = new List<string>();

        public void AddError(string error)
        {
            _errors.Add(error);
        }

        public bool HasError => _errors.Any();

        public IReadOnlyList<string> Errors => _errors;
    }

}
