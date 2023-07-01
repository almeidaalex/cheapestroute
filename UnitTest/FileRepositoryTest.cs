using Core;

using NUnit.Framework;

using System;

namespace UnitTest;

public class FileRepositoryTest
{
  [Test]
  public void Should_read_a_line_and_convert_to_properly_format()
  {
    var repo = new FileRepository();
    repo.AddRoute("GRU", "BRC", 10);
    Assert.AreEqual(1, repo.GetAll().Count);
  }

  [Test]
  public void Should_upper_case_all_routes_inputed()
  {
    var line = "gru,brc,10";

    //var network = new FlightNetwork();
    ///var result = network.LoadFrom(new[] { line });

    // Assert.IsFalse(result.HasErrors);
    // Assert.IsNotNull(network.Hubs["GRU"]);
  }

  [Test]
  public void Should_invalidate_circular_reference()
  {
    var repo = new FileRepository();
    Assert.Throws<ArgumentException>(() => repo.AddRoute("AAA", "AAA", 180));
  }
}

