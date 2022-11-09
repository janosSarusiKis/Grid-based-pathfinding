using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Grid;
using Patfinding;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    // Movement and gird
    private CustomGrid customGrid;
    public Pathfinding pathfinding;

    private Vector2Int? currentHoveredGridPos;
    private List<GameObject> calculatedPath;

    private List<GameObject> walkingPath;
    private bool AtTarget;

    private int treeLayer;

    private void Start()
    {
        setupCustomGrid();
        pathfinding = new Pathfinding(customGrid);
        calculatedPath = new List<GameObject>();
        walkingPath = new List<GameObject>();
        treeLayer = LayerMask.NameToLayer(Constants.TreeLayerName);

        StartCoroutine(MoveTowardsTheTarget());
    }

    private void Update()
    {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, Mathf.Infinity, treeLayer))
        {
            // TODO: There is an IndexOutOfRangeException somewhere with some tiles around the edge
            var gridPosOnMousePos = customGrid.HasTileInWorldPosition(raycastHit.point);
            // Only highlight tile when mouse moves to a new one.
            if (gridPosOnMousePos.HasValue && gridPosOnMousePos != currentHoveredGridPos)
            {
                // If we over a grid we clear old target regardless of new is walkable or not if the pld has a value
                if (currentHoveredGridPos.HasValue)
                {
                    customGrid.ResetPathHighlighting(calculatedPath);
                }
                currentHoveredGridPos = gridPosOnMousePos;
                // If the current tile is walkable
                if (customGrid.TileArray[currentHoveredGridPos.Value.x, currentHoveredGridPos.Value.y].Walkable)
                {
                    var targetTile = currentHoveredGridPos.Value;
                    // If we have NPCs we can check for nearest accesible tile here
                    var path = pathfinding.GetPath(
                        customGrid.GetXY(transform.position),
                        targetTile);

                    calculatedPath = customGrid.HighLightPath(path.Select(path => path.Coordinate).ToList());
                }
            }
        }
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (calculatedPath.Any())
            {
                walkingPath = calculatedPath;
                AtTarget = false;
            }
        }
    }

    // MOVEMENT RELATED SCRIPTS
    IEnumerator MoveTowardsTheTarget()
    {
        for (; ; )
        {
            var timeBetweenSteps = 0.1f;
            yield return new WaitForSeconds(timeBetweenSteps);
            if (enabled && !AtTarget)
                MakeOneStepTowardsTheTarget();
        }
    }

    private void MakeOneStepTowardsTheTarget()
    {
        // Check for NPC here again
        if (walkingPath.Any())
        {
            // If have some animator:
            // animator.SetTrigger("WalkTrigger");
            transform.position = walkingPath.FirstOrDefault().transform.position + new Vector3(1, 0.1f, 1);
            walkingPath.First().GetComponent<MeshFilter>().mesh.Clear();
            walkingPath.RemoveAt(0);
        }
        else
        {
            AtTarget = true;
        }

        // if have  a LOS system:
        // UpdateLineOfSight();
    }

    // setupCustomGrid collect Tiles and build a CustomGrid object so it can be used for Pathfinding and etc.
    private void setupCustomGrid()
    {
        var gridify = GameObject.Find("CustomGrid").GetComponent<Gridify>();
        customGrid = new CustomGrid(gridify.width, gridify.height, gridify.size);
        for (int x = 0; x < gridify.width; x++)
        {
            for (int y = 0; y < gridify.height; y++)
            {
                customGrid.TileArray[x, y] = new CustomTile(
                    new Vector2Int(x, y),
                    GameObject.Find(Constants.TileNamePrefix + x + "," + y).GetComponent<Tile>().Walkable);
            }
        }
    }
}
