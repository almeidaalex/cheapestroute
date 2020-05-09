using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;

public class World
{

    //private IDictionary<Airport, ICollection<Airport>> hubs = new Dictionary<Airport, ICollection<Airport>>();

    //public void AddVertex(Airport airport)
    //{
    //    hubs.Add(airport, new List<Airport>());
    //}

    //public void AddEdge(Airport source, Airport destination)
    //{
    //    if (!hubs.ContainsKey(source))
    //        AddVertex(source);
    //    if (!hubs.ContainsKey(destination))
    //        AddVertex(destination);

    //    hubs[source].Add(destination);
    //    hubs[destination].Add(source);
    //}
}

public class Hub2
{
    public Airport Source { get; set; }
    public Airport Destination { get; set; }
    public int Cost { get; set; }

    public Hub2(Airport source, Airport destination, int cost)
    {
        Source = source;
        Destination = destination;
        Cost = cost;
    }
}

public class Airport
{
    private readonly List<Hub> _list = new List<Hub>();

    public Airport(string name)
    {
        Name = name;
    }

    public string Name { get; private set; }

    public Hub[] Hubs => _list.ToArray();

    public Airport AddHub(Airport airport, int cost)
    {
        var hub = new Hub(airport, cost);        
        _list.Add(hub);


        return this;
    }
}

public class Hub : IComparable
{
    private readonly List<Hub> _list = new List<Hub>();

    public Hub(Airport name, int cost = 0)
    {
        Airport = name;
        Cost = cost;
        TravelCost = int.MaxValue;
    }

    public Hub[] Hubs => _list.ToArray();
    public Airport Airport { get; }

    public int Cost { get; private set; }

    public int TravelCost { get; private set; }

    public Hub LastLeg { get; private set; }

    public bool IsVisited { get; private set; }

    //Methods
    public void AddCost(int cost)
    {
        this.Cost += cost;
    }

    public Hub AddHub(Airport airport, int cost)
    {   
        var hub = new Hub(airport, cost);
        _list.Add(hub);
        return this;
    }

    public Hub SetLastLeg(Hub lastLeg)
    {
        LastLeg = lastLeg;
        return this;
    }

    public int CompareTo(object obj)
    {
        var hub = obj as Hub;
        return this.Cost.CompareTo(hub.Cost);
    }

    internal Hub Visit()
    {
        foreach (var hub in this.Hubs.Where(h => h.IsVisited == false))
        {
            hub.SetLastLeg(this);
            this.AddCost(hub.Cost);            
        }
        this.IsVisited = true;
        return this.Hubs.Min();
    }
}




