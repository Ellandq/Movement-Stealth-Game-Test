using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingHandler : MonoBehaviour
{
    public static PathfindingHandler Instance;

    [Header ("Level Information")]
    [SerializeField] private Transform levelOriginPoint;
    [SerializeField] private List<Transform> roomLocations;
    [SerializeField] private int mapSizeX;
    [SerializeField] private int mapSizeY;
    [SerializeField] private int mapSizeZ;

    [Header ("Movment Grid Settings")]
    [SerializeField] private float xAxisSpacing;
    [SerializeField] private float yAxisSpacing;
    [SerializeField] private float zAxisSpacing;
    [SerializeField] private LayerMask enviromentLayer;


    [Header ("Pathfinding settings")]
    private bool[,,] worldOccupationStatus;

    #region SetUp

    private void Awake (){
        Instance = this;
    }

    private void Start (){
        SetUpPathfindingHandler();
    }

    private void SetUpPathfindingHandler (){
        worldOccupationStatus = new bool[
        Mathf.FloorToInt(mapSizeX / xAxisSpacing),
        Mathf.FloorToInt(mapSizeY / yAxisSpacing),
        Mathf.FloorToInt(mapSizeZ / zAxisSpacing)
        ];

        Vector3 offsetPosition;

        for (int x = 0; x < worldOccupationStatus.GetLength(0); x++){
            for (int y = 0; y < worldOccupationStatus.GetLength(1); y++){
                for (int z = 0; z < worldOccupationStatus.GetLength(2); z++){
                    offsetPosition = new Vector3(x * xAxisSpacing, y * yAxisSpacing, z * zAxisSpacing);
                    worldOccupationStatus[x, y, z] = Physics.CheckSphere(levelOriginPoint.position + offsetPosition, .5f, enviromentLayer);
                    // if (worldOccupationStatus[x, y, z]){
                    //     GameObject.CreatePrimitive(PrimitiveType.Sphere).transform.position = levelOriginPoint.position + offsetPosition;
                    // }
                }
            }
        }
    }

    #endregion

    #region Pathfinding

    public List<Vector3> GetPathToTarget (Vector3 startPos, Vector3 targetPos, float height, float size, bool isFlying){
        short defaultHeight = GetDefaultHeight(GetClosestEmptyGridPositionFromWorldPosition(startPos));
        Node startNode = new Node(GetClosestEmptyGridPositionFromWorldPosition(startPos));
        Node targetNode = new Node(GetClosestEmptyGridPositionFromWorldPosition(targetPos));

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];

            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FCost < currentNode.FCost || (openSet[i].FCost == currentNode.FCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode.position == targetNode.position)
            {
                return RetracePath(startNode, targetNode);
            }

            foreach (Node neighbor in GetNeighbors(currentNode, defaultHeight, height, size, isFlying))
            {
                if (closedSet.Contains(neighbor))
                {
                    continue;
                }

                float newGCost = currentNode.gCost + Vector3.Distance(currentNode.position, neighbor.position);
                if (newGCost < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newGCost;
                    neighbor.hCost = Vector3.Distance(neighbor.position, targetNode.position);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }
        return null;
    }

    private List<Node> GetNeighbors(Node node, short defaultHeight, float height, float size, bool isFlying)
    {
        List<Node> neighbors = new List<Node>();

        int x = node.position.x;
        int z = node.position.z;

        // Define the possible directions to check for neighbors (8 possible directions)
        int[] dx = { -1, 0, 1, -1, 1, -1, 0, 1 };
        int[] dz = { -1, -1, -1, 0, 0, 1, 1, 1 };

        for (int i = 0; i < 8; i++)
        {
            int newX = x + dx[i];
            int newZ = z + dz[i];

            // Check if the new coordinates are within the bounds of your grid
            if (newX >= 0 && newX < mapSizeX && newZ >= 0 && newZ < mapSizeZ)
            {
                PositionValidity positionValidity = IsPositionValidToMove(new Vector3Int(newX, defaultHeight, newZ), node.position, defaultHeight);

                if (positionValidity == PositionValidity.False) continue;

                Node neighborNode = new Node(new Vector3Int(newX, defaultHeight + (int)positionValidity, newZ));

                neighbors.Add(neighborNode);
            }
        }

        return neighbors;
    }


    private List<Vector3> RetracePath(Node startNode, Node endNode)
    {
        List<Vector3> path = new List<Vector3>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode.position);
            currentNode = currentNode.parent;
        }

        path.Reverse();
        return path;
    }

    #endregion

    #region Getters

    private Vector3Int GetClosestEmptyGridPositionFromWorldPosition (Vector3 worldPosition){
        Vector3Int localPosition;
        localPosition = new Vector3Int(
            Mathf.FloorToInt(worldPosition.x / xAxisSpacing),
            Mathf.FloorToInt(worldPosition.y / yAxisSpacing),
            Mathf.FloorToInt(worldPosition.z / zAxisSpacing)
        );
        return localPosition;
    }

    private Vector3 GetWorldPositionFromGrid (Vector3Int gridPosition){
        Vector3 worldPosition = new Vector3(
            gridPosition.x * xAxisSpacing,
            gridPosition.y * yAxisSpacing,
            gridPosition.z * zAxisSpacing
        );

        return worldPosition;
    }

    private short GetDefaultHeight (Vector3 worldPosition){
        Vector3 gridPosition = GetClosestEmptyGridPositionFromWorldPosition(worldPosition);
        short defaultHeight = 0;
        for (int i = (int)worldPosition.y; i > 0; i--){
            if (worldOccupationStatus[(int)worldPosition.x, i, (int)worldPosition.z]) return defaultHeight;
            else defaultHeight++;
        }
        return defaultHeight;
    }

    private PositionValidity IsPositionValidToMove (Vector3Int targetPosition, Vector3Int currentPosition, short defaultHeight){
        for (int i = 0; i < defaultHeight; i++){
            if (worldOccupationStatus[targetPosition.x, targetPosition.y - i, targetPosition.z]){
                return PositionValidity.False;
            }
        }
        if    (worldOccupationStatus[targetPosition.x, targetPosition.y - defaultHeight, targetPosition.z]
        && !worldOccupationStatus[targetPosition.x, targetPosition.y + 1, targetPosition.z])
        {
            return PositionValidity.UpOne;
        }
        else if    (!worldOccupationStatus[targetPosition.x, targetPosition.y - defaultHeight, targetPosition.z]
        && !worldOccupationStatus[targetPosition.x, targetPosition.y - (defaultHeight + 1), targetPosition.z])
        {
            return PositionValidity.DownOne;
        }
        else
        {
            return PositionValidity.True;
        }
    }

    #endregion

    private enum PositionValidity{
        True = 0, False = 2,
        DownOne = -1, UpOne = 1
    }
}


