using CheapestTravel;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

public class ShortPathTest
{

    [Test]
    public void Should_find_shortest_path_from_a_valid_graph()
    {
        var network = new FlightNetwork();
        var lines = new[] {
            "GRU,BRC,10",
            "BRC,SCL,5",
            "GRU,CDG,75",
            "GRU,SCL,20",
            "GRU,ORL,56",
            "ORL,CDG,5",
            "SCL,ORL,20"
            };

        network.LoadFrom(lines);

        var cheapestRoute = network.CheapRoute("BRC", "CDG");

        Assert.IsFalse(cheapestRoute.HasErrors);
        Assert.AreEqual(30, cheapestRoute.Cost);
        Assert.AreEqual("BRC - SCL - ORL - CDG > $30", cheapestRoute.ToString());
    }

    [Test]
    public void Should_return_error_when_origin_dont_exists()
    {
        var network = new FlightNetwork();
        var lines = new[] {
            "LAX,BRC,10",
            "BRC,SCL,5",
            "LAX,CDG,75",
            "LAX,SCL,20",
            "LAX,ORL,56",
            "ORL,CDG,5",
            "SCL,ORL,20"
            };

        network.LoadFrom(lines);

        var cheapestRoute = network.CheapRoute("GRU", "CDG");

        Assert.IsTrue(cheapestRoute.HasErrors);
        Assert.AreEqual("NO_ORIGIN_FOUND > GRU", cheapestRoute.Errors.First());
    }


    [Test]
    public void Should_return_error_when_destination_dont_exists()
    {
        var network = new FlightNetwork();
        var lines = new[] {
            "LAX,BRC,10",
            "BRC,SCL,5",
            "LAX,CDG,75",
            "LAX,SCL,20",
            "LAX,ORL,56",
            "ORL,CDG,5",
            "SCL,ORL,20"
            };
        network.LoadFrom(lines);

        var cheapestRoute = network.CheapRoute("LAX", "WOW");
        Assert.IsTrue(cheapestRoute.HasErrors);
        Assert.AreEqual("NO_DESTINATION_FOUND > WOW", cheapestRoute.Errors.First());
    }


    [Test]
    public void Should_return_error_when_there_arent_connection_between_points()
    {
        var network = new FlightNetwork();
        var lines = new[] {
            "GRU,BRC,10",
            "BRC,SCL,5",
            "LAX,CDG,75"
            };

        network.LoadFrom(lines);

        var cheapestRoute = network.CheapRoute("GRU", "CDG");

        Assert.IsTrue(cheapestRoute.HasErrors);
        Assert.AreEqual("NO_ROUTE", cheapestRoute.Errors.First());
    }
    
}