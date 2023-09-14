using System.Text;
using Microsoft.Extensions.Logging;
using CodingChallenges.RailRoadService.Contracts;

namespace CodingChallenges.RailRoadService.Core
{
    public class RouteProvider : IRouteProvider
    {
        private Node[] _nodes;
        private readonly ILogger _logger;
        private const int MAX_DISTANCE = int.MaxValue;

        public RouteProvider(ILogger<RouteProvider> logger)
        {
            _logger = logger;
        }

        public void Initialize(Node[] routeInfo)
        {
            _nodes = routeInfo;
        }

        public int GetDistanceAlongTheRoute(char[] townNames)
        {
            ValidateRouteInformationIsSet();

            if (townNames.Length <= 1)
            {
                throw new ArgumentException(nameof(townNames), "Invalid parameter. There should be atleast two names of towns.");
            }

            ValidateTownNames(townNames);

            int i = 1, dist = 0;

            var sourceTown = GetNode(townNames[0]);

            if (sourceTown == null)
            {
                throw new ArgumentException("Invalid town name.");
            }

            while (i < townNames.Length)
            {
                var nextTown = GetNode(townNames[i]);

                var found = false;

                foreach (var neighbour in sourceTown.Edges)
                {
                    if (neighbour.Node == nextTown)
                    {
                        found = true;
                        dist += neighbour.Weight;
                        break;
                    }
                }

                if (!found)
                {
                    return -1;
                }

                sourceTown = nextTown;
                i++;
            }

            return dist;
        }

        public int GetNumberOfTripsConstrainedByNumberOfStops(char fromTownName, char toTownName, RouteCriteria criteria)
        {
            ValidateRouteInformationIsSet();

            ValidateTownNames(new[] { fromTownName , toTownName });

            var fromTown = GetNode(fromTownName);
            var toTown = GetNode(toTownName);

            int numberOfRoutes = 0;

            int? maximumNumOfStops = criteria.Operator == Operator.Maximum || criteria.Operator == Operator.Equals ? criteria.NumberOfStops : null;

            GetNumberOfRoutesConstrainedByNumOfStops(fromTown, toTown, (GenerateCriteriaForNumberOfStops(criteria), maximumNumOfStops), 0,
                ref numberOfRoutes, new StringBuilder());

            return numberOfRoutes;
        }

        public int GetNumberOfTripsConstrainedByDistance(char fromTownName, char toTownName, int maximumDistance)
        {
            ValidateRouteInformationIsSet();

            ValidateTownNames(new[] { fromTownName, toTownName });

            var fromTown = GetNode(fromTownName);
            var toTown = GetNode(toTownName);

            int numOfRoutes = 0;

            int distance = 0;

            var stringBuilder = new StringBuilder();
            stringBuilder.Append(fromTown.Value);

            GetNumberOfTripsConstrainedByDistance(fromTown, toTown, maximumDistance, distance, ref numOfRoutes, stringBuilder);

            return numOfRoutes;
        }

        public int GetShortestDistance(char fromTownName, char toTownName)
        {
            ValidateRouteInformationIsSet();

            ValidateTownNames(new[] { fromTownName, toTownName });

            var fromTown = GetNode(fromTownName);
            var toTown = GetNode(toTownName);

            var distances = new int[_nodes.Length];

            foreach (var node in _nodes)
            {
                var index = GetTownNodeIndex(node.Value);
                if (node == fromTown)
                {
                    distances[index] = 0;
                }
                else
                {
                    distances[index] = MAX_DISTANCE;
                }
            }

            var queue = new Queue<Node>();
            queue.Enqueue(fromTown);
            var shortestDistance = MAX_DISTANCE;

            while (queue.Count > 0)
            {
                var currentNode = queue.Dequeue();
                var currentNodeIndex = GetTownNodeIndex(currentNode.Value);

                foreach (var edge in currentNode.Edges)
                {
                    var neighbour = edge.Node;
                    var neighbourIndex = GetTownNodeIndex(neighbour.Value);

                    var newPossibleMin = distances[currentNodeIndex] + edge.Weight;

                    if (newPossibleMin > shortestDistance)
                    {
                        continue;
                    }

                    if ((distances[neighbourIndex] > newPossibleMin) || (neighbour == toTown && distances[neighbourIndex] == 0))
                    {
                        distances[neighbourIndex] = newPossibleMin;
                        queue.Enqueue(neighbour);
                    }

                    if (neighbour == toTown && distances[neighbourIndex] < shortestDistance)
                    {
                        shortestDistance = distances[neighbourIndex];
                    }
                }
            }

            // no route
            if (shortestDistance == MAX_DISTANCE)
            {
                return -1;
            }

            return shortestDistance;
        }

        private void GetNumberOfTripsConstrainedByDistance(Node fromTown, Node toTown, int maximumDistance, int distance, ref int numRoutes, StringBuilder currentPath)
        {
            foreach (var neighbour in fromTown.Edges)
            {
                if (distance + neighbour.Weight >= maximumDistance)
                {
                    continue;
                }

                currentPath.Append(neighbour.Node.Value);

                if (neighbour.Node == toTown)
                {
                    _logger.LogInformation("GetNumberOfTripsConstrainedByDistance: Valid route found: {route}\n", currentPath.ToString());
                    numRoutes++;
                }

                var index = GetTownNodeIndex(neighbour.Node.Value);

                var maxDist = maximumDistance - neighbour.Weight;
                int localNum = 0;

                GetNumberOfTripsConstrainedByDistance(neighbour.Node, toTown, maxDist, distance, ref localNum, currentPath);
                numRoutes += localNum;
                currentPath.Remove(currentPath.Length - 1, 1);
            }
        }

        private Func<int, bool> GenerateCriteriaForNumberOfStops(RouteCriteria criteria)
        {
            if (criteria.Operator == Operator.Equals)
            {
                return (numStops) => numStops == criteria.NumberOfStops;
            }
            else if (criteria.Operator == Operator.Maximum)
            {
                return (numStops) => numStops <= criteria.NumberOfStops;
            }
            else
            {
                return (numStops) => numStops >= criteria.NumberOfStops;
            }
        }

        private void GetNumberOfRoutesConstrainedByNumOfStops(Node fromTown, Node toTown, (Func<int, bool> ConditionForNumberOfStops, int? MaximumNumOfStops) criteria, int numOfStops, ref int numRoutes, StringBuilder currentPath)
        {
            currentPath.Append(fromTown.Value);

            numOfStops++;

            if (criteria.MaximumNumOfStops != null && numOfStops > criteria.MaximumNumOfStops)
            {
                return;
            }

            var numStopsCondition = criteria.ConditionForNumberOfStops;

            foreach (var neighbour in fromTown.Edges)
            {
                if (neighbour.Node == toTown && (numStopsCondition == null || numStopsCondition(numOfStops)))
                {
                    currentPath.Append(toTown.Value);

                    _logger.LogInformation("GetNumberOfRoutesConstrainedByNumOfStops: Valid route found: {route}\n", currentPath.ToString());

                    numRoutes++;
                }
                else
                {
                    GetNumberOfRoutesConstrainedByNumOfStops(neighbour.Node, toTown, criteria, numOfStops, ref numRoutes, currentPath);
                }

                currentPath.Remove(currentPath.Length - 1, 1);
            }
        }

        private Node GetNode(char townName)
        {
            var index = GetTownNodeIndex(townName);
            return _nodes[index];
        }

        // for similicity (as per the nature of inputs), it is assumed that inputs are always uppercase alphabets.
        private static int GetTownNodeIndex(char townName)
        {
            return townName - 'A';
        }

        // Expectation is that town names are upper case alphabet
        private static void ValidateTownNames(char[] townNames)
        {
            foreach(var townName in townNames)
            {
                if (!char.IsAsciiLetterUpper(townName))
                {
                    throw new ArgumentException($"Invalid town name: {townName}");
                }
            }
        }

        private void ValidateRouteInformationIsSet()
        {
            if (_nodes == null || !_nodes.Any())
            {
                throw new InvalidOperationException("Route Provider is not initialized.");
            }
        }
    }
}
