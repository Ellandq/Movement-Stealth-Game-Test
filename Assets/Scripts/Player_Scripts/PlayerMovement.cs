using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header ("Movement Settings")]
    // Horizontal Movement
    [SerializeField] private float speed = 18f;
    [SerializeField] private float maxSpeed = 35f;
    [SerializeField] private float airControlSpeed = 60f;
    [SerializeField] private float acceleration = 0.5f;
    [SerializeField] private float deceleration = 0.4f;
    // Vertical movement
    [SerializeField] private float jumpHeight = 4f;
    

    [Header ("Player Information")]
    // Movement
    [SerializeField] private float currentVelocity;
    private Vector3 velocity;
    private float accelerationStatus = 0f;
    // Statuses
    [SerializeField] private bool isGrounded;

    [Header ("Object References")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private Transform groundCheck;

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
        isGrounded = controller.isGrounded || Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }

    #endregion

    #region Player Movement

    private void Jump (){
        if (!isGrounded) return;
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);
    }

    // Function handling movement based on user input
    private void UserMovement() {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 baseMovementNormalized = (transform.right * x + transform.forward * z).normalized;

        if (baseMovementNormalized.magnitude <= 0.5f) {
            accelerationStatus = Mathf.MoveTowards(accelerationStatus, 0f, 2f * acceleration * Time.deltaTime);
        } else {
            accelerationStatus = Mathf.MoveTowards(accelerationStatus, 1f, acceleration * Time.deltaTime);
        }

        Vector3 horizontalVelocity = baseMovementNormalized *
            (speed + accelerationStatus * (maxSpeed - speed));

        Vector3 newVelocity;

        if (isGrounded) {
            // Apply gradual deceleration only when grounded
            if (baseMovementNormalized.magnitude <= 0.1f) {
                newVelocity = Vector3.MoveTowards(velocity, Vector3.zero, 250f * deceleration * Time.deltaTime);
            } else {
                newVelocity = Vector3.Lerp(velocity, horizontalVelocity, accelerationStatus);
            }
        } else {
            // Calculate mid-air acceleration based on the current velocity
            Vector3 midAirAcceleration = baseMovementNormalized * airControlSpeed * Time.deltaTime * Mathf.Clamp01(1f - velocity.magnitude / maxSpeed);

            // Allow more control of momentum while in mid-air
            Vector3 playerInputInfluence = baseMovementNormalized * airControlSpeed * Time.deltaTime;

            // Combine adjusted velocity, mid-air acceleration, and player input influence
            newVelocity = velocity + midAirAcceleration + playerInputInfluence;

        }

        // Apply clamping to individual components of newVelocity
        newVelocity = new Vector3(
            Mathf.Clamp(newVelocity.x, -maxSpeed, maxSpeed),
            newVelocity.y,
            Mathf.Clamp(newVelocity.z, -maxSpeed, maxSpeed)
        );

        // Update the actual velocity after all calculations
        velocity.x = newVelocity.x;
        velocity.z = newVelocity.z;
    }

    // Function handling the player gravity
    private void GravityAcceleration (){
        if (!controller.isGrounded){
            velocity.y += Physics.gravity.y * Time.deltaTime;
        }else{
            if (velocity.y < -0.2f && !(velocity.y > 0f)){
                velocity.y = Mathf.Lerp(velocity.y, 0f, 0.5f);
            }
        }
    }
    
    #endregion

    #region Getters

    public bool IsGrounded(){
        return isGrounded;
    }

    public float GetCurrentVelocity(){
        return currentVelocity;
    }

    #endregion
}
