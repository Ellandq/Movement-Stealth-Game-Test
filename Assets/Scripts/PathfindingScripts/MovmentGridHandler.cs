using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovmentGridHandler : MonoBehaviour
{
    public static MovmentGridHandler Instance;

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

    private Vector3 GetClosestEmptyGridPositionFromWorldPosition (Vector3 worldPosition){
        Vector3 localPosition = levelOriginPoint.position - worldPosition;
        localPosition = new Vector3(
            Mathf.Floor(localPosition.x / xAxisSpacing),
            Mathf.Floor(localPosition.y / yAxisSpacing),
            Mathf.Floor(localPosition.z / zAxisSpacing)
        );
        return localPosition;
    }
}
