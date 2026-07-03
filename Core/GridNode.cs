using System;
using System.Xml;
namespace AStarShowcase.Core
{
    public class GridNode : INode
    {
        public int X { get;}
        public int Y {get;}
        public bool IsWalkable { get; set;}
        public float MovementCost { get; set;}
        public GridNode(int x, int y, bool isWalkable = true, float movementCost = 1f)
        {
            X = x;
            Y = y;
            IsWalkable = isWalkable;
            MovementCost = movementCost;
        }
        public override string ToString () => $"({X}, {Y})";
    }
}