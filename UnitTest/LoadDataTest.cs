﻿using CheapestTravel;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class LoadDataTest
{
    [Test]
    public void Should_read_a_line_and_convert_to_properly_format()
    {
        var line = "GRU,BRC,10";

        var network = new FlightNetwork();
        var result = network.LoadFrom(new[] { line });

        Assert.IsFalse(result.HasErrors);
        Assert.IsNotEmpty(network.Hubs);

    }

    [Test]
    public void Should_not_import_lines_with_mismatch_format()
    {
        var line = "GRU,BRC,AAA";

        var network = new FlightNetwork();
        var result = network.LoadFrom(new[] { line });

        Assert.IsTrue(result.HasErrors);

        var expected = "Line [1] input format mismatch 'string, string, uint' > GRU,BRC,AAA";
        Assert.AreEqual(expected, result.Errors.First());
    }

    [Test]
    public void Should_upper_case_all_routes_inputed()
    {
        var line = "gru,brc,10";

        var network = new FlightNetwork();
        var result = network.LoadFrom(new[] { line });

        Assert.IsFalse(result.HasErrors);        
        Assert.IsNotNull(network.Hubs["GRU"]);
    }

    [Test]
    public void Should_invalidate_circular_reference()
    {
        var line = "AAA,AAA,180";

        var network = new FlightNetwork();
        var result = network.LoadFrom(new[] { line });

        Assert.AreEqual("CIRCULAR_REFERENCE > AAA, AAA", result.Errors.First());
    }
}

