using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CodingChallenges.RailRoadService.Contracts;
using CodingChallenges.RailRoadService.Core;

namespace CodingChallenges.RailRoadService.Demo
{
    internal static class RouteProviderDemo
    {
        public static void Run(IHost serviceHost)
        {
            var routeProvider = serviceHost.Services.GetService<IRouteProvider>();

            //var nodes = RouteProcessor.BuildGraph("AB5, BC4, CD8, DC8, DE6, AD5, CE2, EB3, AE7");
            var nodes = GraphUtility.BuildGraph("AB5, BC4, DE6, AD5, CE2, EB3, AE7, CD8, DC8");
            routeProvider.Initialize(nodes);

            Console.WriteLine("1. The distance of the route A-B-C.");
            Console.WriteLine($"\t {routeProvider.GetDistanceAlongTheRoute(new char[] { 'A', 'B', 'C' })}");

            Console.WriteLine("2. The distance of the route A-D.");
            Console.WriteLine($"\t {routeProvider.GetDistanceAlongTheRoute(new char[] { 'A', 'D' })}");

            Console.WriteLine("3. The distance of the route A-D-C.");
            Console.WriteLine($"\t {routeProvider.GetDistanceAlongTheRoute(new char[] { 'A', 'D', 'C' })}");

            Console.WriteLine("4. The distance of the route A-E-B-C-D.");
            Console.WriteLine($"\t {routeProvider.GetDistanceAlongTheRoute(new char[] { 'A', 'E', 'B', 'C', 'D' })}");

            Console.WriteLine("5. The distance of the route A-E-D.");
            Console.WriteLine($"\t {routeProvider.GetDistanceAlongTheRoute(new char[] { 'A', 'E', 'D' })}");

            Console.WriteLine("6. The number of trips starting at C and ending at C with a maximum of 3 stops: {0}\n",
                routeProvider.GetNumberOfTripsConstrainedByNumberOfStops('C', 'C', new RouteCriteria { Operator = Operator.Maximum, NumberOfStops = 3 }));

            Console.WriteLine("7. The number of trips starting at A and ending at C with a maximum of 4 stops: {0}\n",
                routeProvider.GetNumberOfTripsConstrainedByNumberOfStops('A', 'C', new RouteCriteria { Operator = Operator.Equals, NumberOfStops = 4 }));

            Console.WriteLine("8. The length of the shortest route (in terms of distance to travel) from A to C.: {0}\n",
                routeProvider.GetShortestDistance('A', 'C'));

            Console.WriteLine("9. The length of the shortest route (in terms of distance to travel) from B to B: {0}\n",
                routeProvider.GetShortestDistance('B', 'B'));

            Console.WriteLine("10. The number of different routes from C to C with a distance of less than 30: {0}",
                routeProvider.GetNumberOfTripsConstrainedByDistance('C', 'C', 30));

        }
    }
}
