using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CheapestTravel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace API
{
    public class Startup
    {
        private const string CSV_FILE = "input-routes.csv";
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/api/route/{from}/{destination}", async context =>
                {
                    try
                    {
                        var from = context.Request.RouteValues["from"];
                        var destination = context.Request.RouteValues["destination"];


                        var result = CalculateRoute(from.ToString(), destination.ToString());

                        if (result.HasErrors)
                        {
                            await context.BadRequest(result.Errors);
                            return;
                        }

                        await context.Ok(result.ToString());

                    }
                    catch(Exception e)
                    {                        
                        await context.InternalServerError(e);
                    }
                });

                endpoints.MapPost("/api/route", async context =>
                {
                    try
                    {
                        using var newRoute = new StreamReader(context.Request.Body);

                        var result = RouteFile.WriteFile(CSV_FILE, await newRoute.ReadLineAsync());

                        if (result.HasErrors)
                        {
                            await context.BadRequest(result.Errors);
                            return;
                        }

                        await context.Created("Route included successfully!");
                    }
                    catch(Exception e)
                    {
                        await context.InternalServerError(e);
                    }
                });
            });
        }

        private Result CalculateRoute(string from, string to)
        {
            var routes = RouteFile.ReadFile(CSV_FILE);
            var network = new FlightNetwork();
            var loadResult = network.LoadFrom(routes);

            if (loadResult.HasErrors)
                return loadResult;

            return network.CheapRoute(from, to);
        }
        
    }

    static class ResponseExtension
    {
        public static async Task BadRequest(this HttpContext context, IEnumerable<string> errors)
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync(string.Join(";", errors));
        }

        public static async Task Ok(this HttpContext context, string message)
        {
            context.Response.StatusCode = 200;
            await context.Response.WriteAsync(message);
        }

        public static async Task Created(this HttpContext context, string message)
        {
            context.Response.StatusCode = 201;
            await context.Response.WriteAsync(message);
        }

        public static async Task InternalServerError(this HttpContext context, Exception e)
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync($"Unfortunately an unexpected error has occurred. : {e.Message}");
        }
    }
}
