using System;

namespace AStarShowcase.Core
{
    public static class Heuristics
    {
        public static float Manhattan(int dx, int dy) 
        {
            return Math.Abs(dx) + Math.Abs(dy);
        }

        public static float Euclidean(int dx, int dy)
        {
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }

        public static float Diagonal(int dx, int dy)
        {
            float absDx = Math.Abs(dx);
            float absDy = Math.Abs(dy);
            const float straigthCost = 1f;
            const float diagonalCost = 1.41421356f;  // √2
            float straigthSteps = Math.Abs(absDx - absDy);
            float diagonalSteps = Math.Min(absDx, absDy);
            return straigthCost * straigthSteps + diagonalCost * diagonalSteps;
        }

        public static float Chebyshev(int dx, int dy)
        {
            return Math.Max(Math.Abs(dx), Math.Abs(dy));
        }
    }

    public enum HeuristicType
    {
        Manhattan,
        Euclidean,
        Diagonal,
        Chebyshev
    }
}