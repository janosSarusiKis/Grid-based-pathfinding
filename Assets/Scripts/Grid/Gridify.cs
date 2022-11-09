using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Grid
{
    public class Gridify : MonoBehaviour
    {
        [SerializeField]
        public int height = 100;
        [SerializeField]
        public int width = 100;
        [SerializeField]
        public int size = 2;
        [SerializeField]
        public float planeScale = 0.2f;

        // Object on Trees layer
        public GameObject[] treePrefabs;

        [SerializeField]
        public float treeNoiseScale = .05f;

        [SerializeField]
        public float treeDensity = .5f;

        [SerializeField]
        private GameObject customTile;

        public CustomGrid customGrid;

        public void GenerateTiles()
        {
            var parent = transform;
            customGrid = new CustomGrid(width, height, size);
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var currentTilePosition = new Vector3Int((x * size), 0, (y * size));

                    var gO = GameObject.Instantiate(customTile);
                    gO.transform.SetParent(parent);
                    gO.transform.position = currentTilePosition;
                    gO.name = $"Tile - {x},{y}";
                }
            }

            GenerateTrees(customGrid);
        }

        void GenerateTrees(CustomGrid customGrid)
        {
            float[,] noiseMap = new float[width, height];
            (float xOffset, float yOffset) = (Random.Range(-10000f, 10000f), Random.Range(-10000f, 10000f));
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float noiseValue = Mathf.PerlinNoise(x * treeNoiseScale + xOffset, y * treeNoiseScale + yOffset);
                    noiseMap[x, y] = noiseValue;
                }
            }

            // We ignore this layer for raycasting when mouse moves over terrain so player can walk behind a tree
            int LayerIgnoreRaycast = LayerMask.NameToLayer(Constants.TreeLayerName);
            // Fake walkthorugh Grid
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float v = Random.Range(0f, treeDensity);
                    if (noiseMap[x, y] < v)
                    {
                        // Put down game object with some random factors
                        GameObject prefab = treePrefabs[Random.Range(0, treePrefabs.Length)];
                        GameObject tree = Instantiate(prefab, transform);
                        tree.transform.position = new Vector3((size / 2) + (x * size), 0.2f, (size / 2) + (y * size));
                        tree.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);
                        tree.transform.localScale = Vector3.one * Random.Range(.8f, 1.2f);
                        tree.layer = LayerIgnoreRaycast; // Important

                        // Set tile wit tree unaccessable to player
                        customGrid.TileArray[x, y].Walkable = false;
                        GameObject.Find(Constants.TileNamePrefix + x + "," + y).GetComponent<Tile>().Walkable = false;
                    }
                }
            }
        }
    }
}