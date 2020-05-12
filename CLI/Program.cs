using CheapestTravel;
using System;
using System.Text.RegularExpressions;

namespace CLI
{
    class Program
    {
        static void Main(string[] args)
        {

            if (args.Length == 0)
                Console.WriteLine("File with routes must be provided");

            var routes = RouteFile.ReadFile(args[0]);
            var network = new FlightNetwork();
            var loadResult = network.LoadFrom(routes);

            if (loadResult.HasErrors)
                Console.WriteLine(string.Join(Environment.NewLine, loadResult.Errors));
            else
                while (true)
                {
                    Console.Write("please enter the route: ");
                    var route = Console.ReadLine();

                    if (!ValidateRoute(route))
                    {
                        Console.WriteLine("Invalid route input format, you should try AAA-AAA");
                        continue;
                    }

                    var splitedRoutes = route.Split("-");
                    var routeResult = network.CheapRoute(splitedRoutes[0], splitedRoutes[1]);
                    if (routeResult.HasErrors)
                        Console.WriteLine(string.Join(Environment.NewLine, routeResult.Errors));
                    else
                        Console.WriteLine(routeResult.ToString());
                }

        }

        private static bool ValidateRoute(string givenRoute)
        {
            var regex = new Regex("[a-zA-Z]{1,}-[a-zA-Z]{1,}");
            return regex.IsMatch(givenRoute);
        }      

    }
}
