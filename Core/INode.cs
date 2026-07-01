using System.Collections.Generic;

namespace AStarShowcase.Core
{
    public interface INode
    {
        bool IsWalkable { get;}
        float MovementCost { get;} 
    }
}