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
    private short currentPathPoint;
    private List<Vector3> currentPath;

    [Header ("Coroutines")]
    private Coroutine currentAction;

    private void Start (){
        currentPath = new List<Vector3>();
        ActivateEnemy();
        // Debug.Log("ACTUAL POSITION: " + transform.position);
        // Debug.Log("LOCAL POSITION: " + transform.localPosition);
    }

    private void ActivateEnemy (){
        if (pathLocationID.Count > 1){
            currentState = EnemyState.Roaming;
            currentPathPoint = 0;
            CheckForNextAction();
        }else{
            currentState = EnemyState.Waiting;
            currentPathPoint = 0;
        }
    }

    private void CheckForNextAction (){
        currentAction = null;
        switch (currentState){
            case EnemyState.Roaming:
                currentPath = PathfindingHandler.Instance.GetPathToTarget(transform.position, pathLocationID[currentPathPoint], 2f, 1.5f, isCapableOfFlight);
                Debug.Log(currentPath);
                currentAction = StartCoroutine(MovementAction());
                break;
            case EnemyState.Chasing:

                break;
            case EnemyState.Searching:

                break;
            case EnemyState.Incapacitated:

                break;
            default:
                break;
        }
    }

    private void MovementFinished (){
        if (pathStopWaitTime[currentPathPoint] != 0){
            currentState = EnemyState.Waiting;
            currentAction = StartCoroutine(WaitAction(pathStopWaitTime[currentPathPoint]));
        }else{
            WaitingFinished();
        }
    }

    private void WaitingFinished (){
        if (currentPathPoint + 1 == pathLocationID.Count) currentPathPoint = 0;
        else currentPathPoint++;
        currentState = EnemyState.Roaming;
        CheckForNextAction();
    }

    private IEnumerator MovementAction (){
        for (int i = 0; i < currentPath.Count; i++){
            while (transform.position != currentPath[i]){
                transform.position = Vector3.MoveTowards(transform.position, currentPath[i], defaultMovementSpeed * Time.deltaTime);
                yield return null;
            }
        }
        MovementFinished();
    }

    private IEnumerator WaitAction (float waitTime){
        // TODO implement waiting animation, looking around etc
        yield return new WaitForSeconds(waitTime);

        WaitingFinished();
    }
}

public enum EnemyState{
    Waiting, Roaming, Chasing, Searching, 
    Incapacitated, Destroyed, 
    Asleep, ShuttingDown, StartingUp
}
