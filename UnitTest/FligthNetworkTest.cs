using CheapestTravel;

using Core;

using Moq;

using NUnit.Framework;

using System;
using System.Collections.Generic;
using System.Linq;

namespace UnitTest;

public class FligthNetworkTest
{

  [Test]
  public void Should_find_shortest_path_from_a_valid_graph()
  {
    var lines = new[] {
            "GRU,BRC,10",
            "BRC,SCL,5",
            "GRU,CDG,75",
            "GRU,SCL,20",
            "GRU,ORL,56",
            "ORL,CDG,5",
            "SCL,ORL,20"
    };

    Mock<IRoutesRepository> repo = new();
    repo.Setup(r => r.GetAll()).Returns(() => GetRoutes(lines));
    var network = new FlightNetwork(repo.Object);

    var cheapestRoute = network.CheapRoute("BRC", "CDG");

    Assert.IsFalse(cheapestRoute.HasErrors);
    Assert.AreEqual(30, cheapestRoute.Cost);
    Assert.AreEqual("BRC - SCL - ORL - CDG > $30", cheapestRoute.ToString());
  }

  [Test]
  public void Should_return_error_when_origin_dont_exists()
  {
    var lines = new[] {
      "LAX,BRC,10",
      "BRC,SCL,5",
      "LAX,CDG,75",
      "LAX,SCL,20",
      "LAX,ORL,56",
      "ORL,CDG,5",
      "SCL,ORL,20"
    };

    Mock<IRoutesRepository> repo = new();
    repo.Setup(r => r.GetAll()).Returns(() => GetRoutes(lines));
    var network = new FlightNetwork(repo.Object);

    var cheapestRoute = network.CheapRoute("GRU", "CDG");

    Assert.IsTrue(cheapestRoute.HasErrors);
    Assert.AreEqual("NO_ORIGIN_FOUND > GRU", cheapestRoute.Errors[0]);
  }


  [Test]
  public void Should_return_error_when_destination_dont_exists()
  {
     var lines = new[] {
            "LAX,BRC,10",
            "BRC,SCL,5",
            "LAX,CDG,75",
            "LAX,SCL,20",
            "LAX,ORL,56",
            "ORL,CDG,5",
            "SCL,ORL,20"
            };

    Mock<IRoutesRepository> repo = new();
    repo.Setup(r => r.GetAll()).Returns(() => GetRoutes(lines));
    var network = new FlightNetwork(repo.Object);

    var cheapestRoute = network.CheapRoute("LAX", "WOW");
    Assert.IsTrue(cheapestRoute.HasErrors);
    Assert.AreEqual("NO_DESTINATION_FOUND > WOW", cheapestRoute.Errors[0]);
  }

  [Test]
  public void Should_return_error_when_there_arent_connection_between_points()
  {
    var lines = new[] {
        "GRU,BRC,10",
        "BRC,SCL,5",
        "LAX,CDG,75"
    };

    Mock<IRoutesRepository> repo = new();
    repo.Setup(r => r.GetAll()).Returns(() => GetRoutes(lines));
    var network = new FlightNetwork(repo.Object);

    var cheapestRoute = network.CheapRoute("GRU", "CDG");

    Assert.IsTrue(cheapestRoute.HasErrors);
    Assert.AreEqual("NO_ROUTE", cheapestRoute.Errors[0]);
  }

  private static IReadOnlyDictionary<string, IReadOnlyDictionary<string, uint>> GetRoutes(string[] lines)
  {
    var dic = new Dictionary<string, Dictionary<string, uint>>();

    Action<string, string, uint> addBidirecionalHub = (string pointA, string pointB, uint cost) => {

      if (dic.ContainsKey(pointA))
        dic[pointA].Add(pointB, cost);
      else
        dic[pointA] = new Dictionary<string, uint> { { pointB, cost } };
    };

    foreach (var line in lines)
    {
      var info = line.Split(',');
      addBidirecionalHub(info[0], info[1], Convert.ToUInt32(info[2]));
      addBidirecionalHub(info[1], info[0], Convert.ToUInt32(info[2]));
    }

    return dic.ToDictionary(h => h.Key, h => (IReadOnlyDictionary<string, uint>)h.Value);
  }
}
