
using FluentAssertions;
using Xunit;

namespace CodingChallenges.RailRoadService.Core.UnitTests
{
    public class GraphUtilityTests
    {
        [Fact]
        public void BuildGraph_IfInputStringIsNull_ThrowsArgumentNullException()
        {
            Action action = () => GraphUtility.BuildGraph(null);
            action
                .Should()
                .Throw<ArgumentNullException>("routeInfo");
        }

        [Fact]
        public void BuildGraph_IfInputDoesNotHaveNodeInfoInExpectedFormat_ThrowsArgumentException()
        {
            Action action = () => GraphUtility.BuildGraph("ABC,BC8");
            action
                .Should()
                .Throw<ArgumentException>("routeInfo");
        }

        [Fact]
        public void BuildGraph_CreatesGraphFromGivenNodesInfoInTheString()
        {
            var routesBetweenTowns = "AB5, BC4, CD8, DC8, DE6, AD5, CE2, EB3, AE7";

            var nodes = GraphUtility.BuildGraph(routesBetweenTowns);

            nodes.Length.Should().Be(5);
            nodes.First(x => x.Value == 'A').Edges.Count.Should().Be(3);
            nodes.First(x => x.Value == 'B').Edges.Count.Should().Be(1);
            nodes.First(x => x.Value == 'C').Edges.Count.Should().Be(2);
            nodes.First(x => x.Value == 'D').Edges.Count.Should().Be(2);
            nodes.First(x => x.Value == 'E').Edges.Count.Should().Be(1);
        }
    }
}
