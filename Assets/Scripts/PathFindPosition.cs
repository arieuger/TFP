using UnityEngine;

namespace DefaultNamespace
{
    public class PathFindPosition
    {
        private Vector2 _position;
        
        private float _g; // distancia desde punto inicial
        private float _h; // distancia desde punto final
        public float F => _g + _h; // suma
        public PathFindPosition Previous { get; set; }

        public Vector2 Position
        {
            get => _position;
            set => _position = value;
        }

        public float G
        {
            get => _g;
            set => _g = value;
        }

        public float H
        {
            get => _h;
            set => _h = value;
        }

        public PathFindPosition(Vector2 position, float g, float h)
        {
            _position = position;
            _g = g;
            _h = h;
        }
    }
}