using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Grid;

namespace Patfinding
{
    public class Pathfinding
    {
        public CustomGrid customGrid;

        public Pathfinding() { }
        public Pathfinding(CustomGrid customGrid)
        {
            this.customGrid = customGrid;
        }

        // PATHFINDING RELATED SCRIPTS
        public List<CustomTile> GetPath(Vector2Int start, Vector2Int destination)
        {
            // Debug.Log($"Start: {start}"); 
            //get player and target position in grid coords
            var startNode = new Node2D(start.x, start.y);
            var targetNode = new Node2D(destination.x, destination.y);


            Heap<Node2D> openSet = new Heap<Node2D>(
                customGrid.TileArray.GetLength(0) * customGrid.TileArray.GetLength(1));
            HashSet<Node2D> closedSet = new HashSet<Node2D>();

            openSet.Add(startNode);

            //calculates path for pathfinding
            while (openSet.Count > 0)
            {
                //iterates through openSet and finds lowest FCost
                Node2D currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);

                //If target found, retrace path
                if (currentNode.GridX == targetNode.GridX && currentNode.GridY == targetNode.GridY)
                {
                    return RetracePath(startNode, closedSet.Last());
                }

                //adds neighbor nodes to openSet
                foreach (var neighbour in GetNeighbours(new Vector2Int(currentNode.GridX, currentNode.GridY)))
                {
                    if (closedSet.Any(x => x.GridX == neighbour.GridX && x.GridY == neighbour.GridY))
                    {
                        continue;
                    }

                    var newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                    if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                        }
                    }
                }
            }

            return new List<CustomTile>();
        }

        public List<Node2D> GetNeighbours(Vector2Int tile)
        {
            var neighbours = new List<Node2D>();
            var coord = new Vector2Int();
            // Here NPC coordinates and other non accessable areas can be exclude
            coord = new Vector2Int(tile.x, tile.y - 1);
            if (customGrid.Walkable(coord))
            {
                neighbours.Add(new Node2D(coord.x, coord.y));
            }
            coord = new Vector2Int(tile.x, tile.y + 1);
            if (customGrid.Walkable(coord))
            {
                neighbours.Add(new Node2D(coord.x, coord.y));
            }

            coord = new Vector2Int(tile.x + 1, tile.y);
            if (customGrid.Walkable(coord))
            {
                neighbours.Add(new Node2D(coord.x, coord.y));
            }
            coord = new Vector2Int(tile.x + 1, tile.y - 1);
            if (customGrid.Walkable(coord))
            {
                neighbours.Add(new Node2D(coord.x, coord.y));
            }
            coord = new Vector2Int(tile.x + 1, tile.y + 1);
            if (customGrid.Walkable(coord))
            {
                neighbours.Add(new Node2D(coord.x, coord.y));
            }

            coord = new Vector2Int(tile.x - 1, tile.y);
            if (customGrid.Walkable(coord))
            {
                neighbours.Add(new Node2D(coord.x, coord.y));
            }
            coord = new Vector2Int(tile.x - 1, tile.y - 1);
            if (customGrid.Walkable(coord))
            {
                neighbours.Add(new Node2D(coord.x, coord.y));
            }
            coord = new Vector2Int(tile.x - 1, tile.y + 1);
            if (customGrid.Walkable(coord))
            {
                neighbours.Add(new Node2D(coord.x, coord.y));
            }

            return neighbours;
        }

        public List<Vector2Int> GetNeighboursInVector2Int(Vector2Int tile)
        {
            var neighbours = new List<Vector2Int>();
            var coord = new Vector2Int();
            // TODO later
            var npcCoordinates = new List<Vector2Int>();
            coord = new Vector2Int(tile.x, tile.y - 1);
            if (customGrid.Walkable(coord))
            {
                neighbours.Add(coord);
            }
            coord = new Vector2Int(tile.x, tile.y + 1);
            if (customGrid.Walkable(coord))
            {
                neighbours.Add(coord);
            }

            coord = new Vector2Int(tile.x + 1, tile.y);
            if (customGrid.Walkable(coord))
            {
                neighbours.Add(coord);
            }
            coord = new Vector2Int(tile.x + 1, tile.y - 1);
            if (customGrid.Walkable(coord))
            {
                neighbours.Add(coord);
            }
            coord = new Vector2Int(tile.x + 1, tile.y + 1);
            if (customGrid.Walkable(coord))
            {
                neighbours.Add(coord);
            }

            coord = new Vector2Int(tile.x - 1, tile.y);
            if (customGrid.Walkable(coord))
            {
                neighbours.Add(coord);
            }
            coord = new Vector2Int(tile.x - 1, tile.y - 1);
            if (customGrid.Walkable(coord))
            {
                neighbours.Add(coord);
            }
            coord = new Vector2Int(tile.x - 1, tile.y + 1);
            if (customGrid.Walkable(coord))
            {
                neighbours.Add(coord);
            }

            return neighbours;
        }

        // Reverses calculated path so first node is closest to seeker and convert to CustomTile
        public List<CustomTile> RetracePath(Node2D startNode, Node2D endNode)
        {
            var path = new List<Node2D>();
            Node2D currentNode = endNode;

            while (currentNode.GridX != startNode.GridX || currentNode.GridY != startNode.GridY)
            {
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }
            path.Reverse();

            var pathInTiles = new List<CustomTile>();
            foreach (var p in path)
            {
                pathInTiles.Add(new CustomTile(new Vector2Int(p.GridX, p.GridY)));
            }

            return pathInTiles;
        }

        // Gets distance between 2 nodes for calculating cost
        public int GetDistance(Node2D nodeA, Node2D nodeB)
        {
            int dstX = Mathf.Abs(nodeA.GridX - nodeB.GridX);
            int dstY = Mathf.Abs(nodeA.GridY - nodeB.GridY);

            if (dstX > dstY)
                return 14 * dstY + 10 * (dstX - dstY);
            return 14 * dstX + 10 * (dstY - dstX);
        }

        public int GetDistance(Vector2Int nodeA, Vector2Int nodeB)
        {
            int dstX = Mathf.Abs(nodeA.x - nodeB.x);
            int dstY = Mathf.Abs(nodeA.y - nodeB.y);

            if (dstX > dstY)
                return 14 * dstY + 10 * (dstX - dstY);
            return 14 * dstX + 10 * (dstY - dstX);
        }

        public Vector2Int GetClosest(Vector2Int start, Vector2Int destination)
        {
            var closest = new Vector2Int();
            var neigbours = GetNeighboursInVector2Int(destination);
            if (neigbours.Any())
            {
                closest = neigbours.OrderBy(x => GetDistance(start, x)).FirstOrDefault();
            }

            return closest;
        }
    }
}