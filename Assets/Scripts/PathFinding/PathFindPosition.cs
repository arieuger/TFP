using UnityEngine;

namespace PathFinding
{
    public class PathFindPosition
    {
        public float G { get; set; } // distancia desde punto inicial
        public float H { get; set; } // distancia desde punto final
        public float F => G + H; // suma
        public PathFindPosition Previous { get; set; }
        public Vector2 Position { get; set; }

        public PathFindPosition(Vector2 position, float g, float h)
        {
            Position = position;
            G = g;
            H = h;
        }
    }
}
