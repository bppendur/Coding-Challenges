namespace CodingChallenges.RailRoadService.Contracts
{
    public class Node
    {
        public char Value { get; set; }

        public List<Edge> Edges { get; set; } = new List<Edge>();
    }
}
