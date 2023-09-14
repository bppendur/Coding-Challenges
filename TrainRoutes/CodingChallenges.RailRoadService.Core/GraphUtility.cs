using System.Text.RegularExpressions;
using CodingChallenges.RailRoadService.Contracts;

namespace CodingChallenges.RailRoadService.Core
{
    public class GraphUtility
    {
        private const char DELIMITER = ',';

        // from the problem definition, town names are expected to be between A - E. If they are more, this can be changed accordingly (ex: maximum of 26, for townnames A - Z).
        private const int TOTAL_NUMBER_OF_TOWNS = 5;

        /// <summary>
        /// Builds graph from route information given in the string. 
        /// Towns information is expected in the following format: ex: AB5, BC4, CD8, DC8, DE6, AD5, CE2, EB3, AE7
        /// </summary>
        /// <param name="routeInfo"></param>
        /// <returns>returns route information betwen towns built as collection of nodes.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static Node[] BuildGraph(string routeInfo)
        {
            if (string.IsNullOrEmpty(routeInfo))
            {
                throw new ArgumentNullException(nameof(routeInfo), "Invalid route information");
            }

            var regex = new Regex(@"([A-Z][A-Z][1-9])");

            var routes = routeInfo.Split(DELIMITER);
            if (!routes.Any())
            {
                throw new ArgumentException(nameof(routeInfo), "Given route info doesn't contain any routes");
            }

            var nodes = new Node[TOTAL_NUMBER_OF_TOWNS];

            foreach (var route in routes)
            {
                if (!regex.IsMatch(route))
                {
                    throw new ArgumentException(nameof(routeInfo), "Invalid route information");
                }

                var route1 = route.Trim();

                var fromTownName = route1[0];
                var toTownName = route1[1];
                var distance = route1[2] - 48;

                Node fromNode = nodes[GetTownIndex(fromTownName)];
                if (fromNode == null)
                {
                    fromNode = new Node() { Value = fromTownName, Edges = new List<Edge>() };
                    nodes[GetTownIndex(fromTownName)] = fromNode;
                }

                Node toNode = nodes[GetTownIndex(toTownName)]; ;
                if (toNode == null)
                {
                    toNode = new Node() { Value = toTownName, Edges = new List<Edge>() };
                    nodes[GetTownIndex(toTownName)] = toNode;
                }

                fromNode.Edges.Add(new Edge { Weight = distance, Node = toNode });
            }

            return nodes;
        }

        private static int GetTownIndex(char townName)
        {
            return townName - 'A';
        }
    }
}
