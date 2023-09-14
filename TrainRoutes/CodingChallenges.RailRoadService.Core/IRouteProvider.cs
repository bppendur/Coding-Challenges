using CodingChallenges.RailRoadService.Contracts;

namespace CodingChallenges.RailRoadService.Core
{
    public interface IRouteProvider
    {
        /// <summary>
        /// Initializes the provider with the routes info.
        /// </summary>
        /// <param name="routeInfo">The collection of nodes</param>
        public void Initialize(Node[] routeInfo);

        /// <summary>
        /// Returns the distance along the given route.
        /// </summary>
        /// <param name="townNames"></param>
        /// <returns>Distance along the given route</returns>
        public int GetDistanceAlongTheRoute(char[] townNames);

        /// <summary>
        /// Gets number of trips from source town to destination town as per given criteria 
        /// </summary>
        /// <param name="fromTownName">source town name</param>
        /// <param name="toTownName">destimation town name </param>
        /// <param name="criteria">criteria of number stops to filter/constrain the routes</param>
        /// <returns>The number of trips possible as per given condition.</returns>
        public int GetNumberOfTripsConstrainedByNumberOfStops(char fromTownName, char toTownName, RouteCriteria criteria);

        /// <summary>
        /// Gets number of trips from source town to destination town as per maximum distance
        /// </summary>
        /// <param name="fromTownName">source town name</param>
        /// <param name="toTownName">destimation town name</param>
        /// <param name="maximumDistance">max number of distance of any route</param>
        /// <returns>the number of trips possible as per the maximum distance constraint.</returns>
        public int GetNumberOfTripsConstrainedByDistance(char fromTownName, char toTownName, int maximumDistance);

        /// <summary>
        /// Gets the shortest distance between from source town to destination town.
        /// </summary>
        /// <param name="fromTownName">source town name</param>
        /// <param name="toTownName">destimation town name</param>
        /// <returns>short path from source town to the destination town</returns>
        public int GetShortestDistance(char fromTownName, char toTownName);
    }
}
