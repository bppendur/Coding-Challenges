using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using CodingChallenges.RailRoadService.Contracts;
using CodingChallenges.RailRoadService.Core;
using Xunit;

namespace CodingChallenges.RailRoadService.Core.UnitTests
{ 
    public class RouteProviderTests
    {
        private IFixture _fixture = null;

        public RouteProviderTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
        }

        [Fact]
        public void GetDistanceAlongTheRoute_IfInputTownNamesArrayIsEmpty_ThrowsArgumentException()
        {
            var routeProvider = _fixture.Create<RouteProvider>();

            routeProvider.Invoking(x => x.GetDistanceAlongTheRoute(Array.Empty<char>()))
                .Should()
                .Throw<ArgumentException>("townNames");
        }

        [Theory]
        [InlineData("A-B-C", 9)]
        [InlineData("A-D", 5)]
        [InlineData("A-D-C", 13)]
        [InlineData("A-E-B-C-D", 22)]
        [InlineData("A-E-D", -1)]
        public void GetDistanceAlongTheRoute_CalculatesCorrectDistance(string routePath, int expectedDistance)
        {
            var routes = GraphUtility.BuildGraph("AB5, BC4, CD8, DC8, DE6, AD5, CE2, EB3, AE7");

            var routeProvider = _fixture.Create<RouteProvider>();
            routeProvider.Initialize(routes);

            var str = routePath.Split('-').Select(x => x[0]);

            var distance = routeProvider.GetDistanceAlongTheRoute(str.ToArray());
            distance.Should()
                .Be(expectedDistance);
        }

        [Theory]
        [InlineData('A', 'C', 4, Operator.Equals, 3)]
        [InlineData('A', 'C', 4, Operator.Maximum, 4)]
        [InlineData('C', 'C', 3, Operator.Maximum, 2)]
        public void GetNumberOfTripsConstrainedByNumberOfStops_ReturnsCorrectNumberOfTrips(char source, char destination, int numberOfStops, Operator conditionOperator, int expectedNumberOfTrips)
        {
            var routes = GraphUtility.BuildGraph("AB5, BC4, CD8, DC8, DE6, AD5, CE2, EB3, AE7");

            var routeProvider = _fixture.Create<RouteProvider>();
            routeProvider.Initialize(routes);

            routeProvider.GetNumberOfTripsConstrainedByNumberOfStops(source, destination, new RouteCriteria { Operator = conditionOperator, NumberOfStops = numberOfStops })
                .Should()
                .Be(expectedNumberOfTrips);
        }

        [Theory]
        [InlineData('C', 'C', 30, 7)]
        [InlineData('C', 'C', 20, 3)]
        [InlineData('D', 'A', 50, 0)]
        public void GetNumberOfTripsConstrainedByDistance_ReturnsCorrectNumberOfTrips(char source, char destination, int maximumDistance, int expectedNumberOfTrips)
        {
            var routes = GraphUtility.BuildGraph("AB5, BC4, CD8, DC8, DE6, AD5, CE2, EB3, AE7");

            var routeProvider = _fixture.Create<RouteProvider>();
            routeProvider.Initialize(routes);

            routeProvider.GetNumberOfTripsConstrainedByDistance(source, destination, maximumDistance)
                .Should()
                .Be(expectedNumberOfTrips);
        }

        [Theory]
        [InlineData('A', 'C', 9)]
        [InlineData('C', 'C', 9)]
        [InlineData('D', 'A', -1)] //no route from 'D' to 'A'
        public void GetShortestDistance_ReturnsCorrectShortestDistanceBetweenTheGivenStations(char source, char destination, int expectedShortDistance)
        {
            var routes = GraphUtility.BuildGraph("AB5, BC4, CD8, DC8, DE6, AD5, CE2, EB3, AE7");

            var routeProvider = _fixture.Create<RouteProvider>();
            routeProvider.Initialize(routes);

            routeProvider.GetShortestDistance(source, destination)
                .Should()
                .Be(expectedShortDistance);
        }
    }
}

