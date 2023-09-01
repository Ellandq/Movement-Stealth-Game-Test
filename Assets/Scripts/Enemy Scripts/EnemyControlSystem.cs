using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControlSystem : MonoBehaviour
{
    [Header ("Movment Settings")]
    [SerializeField] private bool isCapableOfFlight;
    [SerializeField] private float defaultMovementSpeed;
    [SerializeField] private float runningMovementSpeed;

    [Header ("State information")]
    [SerializeField] private EnemyState currentState;
    [SerializeField] private bool isRunning;

    [Header ("Roaming Information")]
    [SerializeField] private List<short> pathLocationID;
    [SerializeField] private List<float> pathStopWaitTime;

}

public enum EnemyState{
    Waiting, Roaming, OnSetPath, Chasing, Searching, 
    Incapacitated, Destroyed, 
    Asleep, ShuttingDown, StartingUp
}
