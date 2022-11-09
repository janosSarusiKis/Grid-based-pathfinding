using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Grid
{
    public class CustomGrid
    {
        private int Width;
        private int Height;
        private float CellSize;
        public CustomTile[,] TileArray;

        public CustomGrid(int width, int height, float cellSize)
        {
            Width = width;
            Height = height;
            CellSize = cellSize;

            TileArray = new CustomTile[width, height];
            for (int x = 0; x < TileArray.GetLength(0); x++)
            {
                for (int y = 0; y < TileArray.GetLength(1); y++)
                {
                    Debug.DrawLine(getWordPositon(x, y), getWordPositon(x, y + 1));
                    Debug.DrawLine(getWordPositon(x, y), getWordPositon(x + 1, y));

                    TileArray[x, y] = new CustomTile(new Vector2Int(x, y));
                }
            }
        }

        // y is turned into z since we want to draw on the terrain plain
        private Vector3 getWordPositon(int x, int y)
        {
            return new Vector3(x, 0, y) * CellSize;
        }

        public void ClickedOnGrid(Vector3 worldPosition)
        {
            var pos = GetXY(worldPosition);
            // This is the GRID coordinate, can be used for grid based pathfinding
            Debug.Log(pos.x + ", " + pos.y);
        }

        // Z is converted to Y so after this visualize the grid as a 2d object
        // Convert a world position into a grid position
        public Vector2Int GetXY(Vector3 worldPosition)
        {
            return new Vector2Int(
                Mathf.FloorToInt(worldPosition.x / CellSize),
                Mathf.FloorToInt(worldPosition.z / CellSize)
            );
        }


        // Retuns the tile coordinate withing the grid if has a tile on position
        public Vector2Int? HasTileInWorldPosition(Vector3 worldPosition)
        {
            var pos = GetXY(worldPosition);
            if (TileArray.GetLength(0) >= pos.x
                && TileArray.GetLength(1) >= pos.y
                && TileArray[pos.x, pos.y] != null)
            {
                return new Vector2Int(pos.x, pos.y);
            }

            return null;
        }

        public void HighLightTargetTile(GameObject gameObject)
        {
            // Full buzi, relativ pozi kell a Tilera
            var x = 0;
            var y = 0;
            var size = 2;
            var sizeOfDrawCell = 2f;
            HighLightTile(gameObject, x, y, sizeOfDrawCell, size);
        }

        public void HighLightPathTile(GameObject gameObject)
        {
            // Use relative positions an sizes for each Tile
            var x = 0.75f;
            var y = 0.75f;
            var size = 2;
            var sizeOfDrawCell = 0.5f;
            HighLightTile(gameObject, x, y, sizeOfDrawCell, size);
        }

        public List<GameObject> HighLightPath(List<Vector2Int> pathTiles)
        {
            var gameObjects = new List<GameObject>();
            if (pathTiles.Any())
            {
                foreach (var pathTile in pathTiles)
                {
                    var gO = GameObject
                            .Find(Constants.TileNamePrefix + pathTile.x + "," + pathTile.y);
                    HighLightPathTile(gO);
                    gameObjects.Add(gO);
                }

                HighLightTargetTile(GameObject.Find(Constants.TileNamePrefix + pathTiles.Last().x + "," + pathTiles.Last().y));
            }

            return gameObjects;
        }

        private void HighLightTile(GameObject gR, float x, float y, float sizeOfDrawCell, float size)
        {
            Mesh mesh = new Mesh();
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            List<Vector2> uvs = new List<Vector2>();
            Vector3 a = new Vector3(x, 0.5f, y + sizeOfDrawCell);
            Vector3 b = new Vector3(x + sizeOfDrawCell, 0.5f, y + sizeOfDrawCell);
            Vector3 c = new Vector3(x, 0.5f, y);
            Vector3 d = new Vector3(x + sizeOfDrawCell, 0.5f, y);
            Vector2 uvA = new Vector2(x / (float)size, y / (float)size);
            Vector2 uvB = new Vector2((x + 1) / (float)size, y / (float)size);
            Vector2 uvC = new Vector2(x / (float)size, (y + 1) / (float)size);
            Vector2 uvD = new Vector2((x + 1) / (float)size, (y + 1) / (float)size);
            Vector3[] v = new Vector3[] { a, b, c, b, d, c };
            Vector2[] uv = new Vector2[] { uvA, uvB, uvC, uvB, uvD, uvC };
            for (int k = 0; k < 6; k++)
            {
                vertices.Add(v[k]);
                triangles.Add(triangles.Count);
                uvs.Add(uv[k]);
            }
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = uvs.ToArray();
            mesh.RecalculateNormals();
            MeshFilter meshFilter = gR.GetComponent<MeshFilter>();
            meshFilter.mesh = mesh;
        }

        public void ResetPathHighlighting(List<GameObject> gameObjects)
        {
            foreach (var gO in gameObjects)
            {
                gO.GetComponent<MeshFilter>().mesh.Clear();
            }
        }

        public bool Walkable(Vector2Int coordinatesInGrid)
        {
            if(coordinatesInGrid.x <0 || coordinatesInGrid.y <0)
            {
                return false;
            }
            return TileArray[coordinatesInGrid.x, coordinatesInGrid.y].Walkable;
        }
    }
}