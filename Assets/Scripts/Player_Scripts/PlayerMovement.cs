using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header ("Movement Settings")]
    // Horizontal Movement
    [SerializeField] private float speed = 12f;
    [SerializeField] private float maxSpeed = 25f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 12f;
    // Vertical movement
    [SerializeField] private float jumpHeight = 4f;
    

    [Header ("Player Information")]
    [SerializeField] private Vector3 velocity; // Remove the serialization later
    [SerializeField] private float currentVelocity;
    [SerializeField] private float accelerationStatus = 0f;
    [SerializeField] private bool isGrounded;

    [Header ("Object References")]
    [SerializeField] private CharacterController controller;

    [Header ("Misc. Settings")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundDistance = 0.4f;

    private void Start (){
        InputManager.Instance.GetKeyboardInputHandler().onJumpButtonPressed += Jump;
    }

    private void Update()
    {
        currentVelocity = new Vector3(velocity.x, 0f, velocity.z).magnitude;

        UpdateGroundedStatus();

        GravityAcceleration();

        UserMovement();

        // Clamping the vertical velocity to the human terminal velocity
        Mathf.Clamp(velocity.y, -55f, 120f);
        controller.Move(velocity * Time.deltaTime);
    }

    #region Player Checks

    private void UpdateGroundedStatus (){
        isGrounded = controller.isGrounded;
    }

    #endregion

    #region Player Movement

    private void Jump (){
        if (!controller.isGrounded) return;
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);
    }

    private void UserMovement (){
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        
        Vector3 baseMovementNormalized = (transform.right * x + transform.forward * z).normalized;

        if (baseMovementNormalized.magnitude <= 0.5f){
            accelerationStatus = Mathf.MoveTowards(accelerationStatus, 0f, 4f * acceleration * Time.deltaTime);
        }else{
            accelerationStatus = Mathf.MoveTowards(accelerationStatus, 1f, acceleration * Time.deltaTime);
        }

        Vector3 horizontalVelocity = baseMovementNormalized * 
        (speed + accelerationStatus * (maxSpeed - speed) - 
        (1 - accelerationStatus) * speed * deceleration);
        
        velocity.x = Mathf.Clamp(horizontalVelocity.x, -maxSpeed, maxSpeed);
        velocity.z = Mathf.Clamp(horizontalVelocity.z, -maxSpeed, maxSpeed);
    }

    private void GravityAcceleration (){
        if (!controller.isGrounded){
            velocity.y += Physics.gravity.y * Time.deltaTime;
        }
    }
    
    #endregion

    #region Getters

    public bool IsGrounded(){
        return controller.isGrounded;
    }

    #endregion
}
