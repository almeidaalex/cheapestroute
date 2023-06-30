using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CheapestTravel
{
    public class FlightNetwork
    {
        public static string LINE_PATTERN = @"[A-Za-z]{1,},[A-Za-z]{1,},\d{1,}";

        private readonly Dictionary<string, Dictionary<string, uint>> _hubs = new Dictionary<string, Dictionary<string, uint>>();

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
            from = from.ToUpper();
            to = to.ToUpper();

            var travelFare = _hubs.ToDictionary(hub => hub.Key, hub => uint.MaxValue);
            var previous = new Dictionary<string, string>();
            var cheapestTravel = new List<string>();
            KeyValuePair<string, uint> visit = new KeyValuePair<string, uint>();

            if (!travelFare.ContainsKey(from))
                return new CheapestRouteResult($"NO_ORIGIN_FOUND > {from}");

            if (!travelFare.ContainsKey(to))
                return new CheapestRouteResult($"NO_DESTINATION_FOUND > {to}");

            travelFare[from] = 0;

            while (travelFare.Any())
            {
                visit = travelFare.OrderBy(r => r.Value).First();

                var hubs = _hubs[visit.Key];
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

        public LoadResult LoadFrom(IEnumerable<string> lines)
        {
            var regex = new Regex(LINE_PATTERN);
            var result = new LoadResult();
            var lineCount = 1;
            foreach (var line in lines)
            {
                if (regex.IsMatch(line))
                {
                    var info = line.Split(",");
                    var source = info[0].ToUpper();
                    var destination = info[1].ToUpper();

                    if (source.Equals(destination))
                    {
                        result.AddError($"CIRCULAR_REFERENCE > {source}, {destination}");
                        continue;
                    }
                    AddHub(source, destination, Convert.ToUInt32(info[2]));
                }
                else
                    result.AddError($"Line [{lineCount}] input format mismatch 'string, string, uint' > {line}");
                lineCount++;
            }
            return result;
        }

        public IReadOnlyDictionary<string, IReadOnlyDictionary<string, uint>> Hubs => _hubs.ToDictionary(h => h.Key, h => (IReadOnlyDictionary<string, uint>)h.Value);
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

    public class LoadResult : Result
    {

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
