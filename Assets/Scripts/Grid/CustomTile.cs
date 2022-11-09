using UnityEngine;

namespace Grid
{
    public class CustomTile
    {
        public Vector2Int Coordinate { get; }

        public bool Walkable { get; set; } = true;

        public CustomTile(Vector2Int coordinateInGrid)
        {
            Coordinate = coordinateInGrid;
        }

        public CustomTile(Vector2Int coordinateInGrid, bool walkable)
        {
            Coordinate = coordinateInGrid;
            Walkable = walkable;
        }
    }
}
