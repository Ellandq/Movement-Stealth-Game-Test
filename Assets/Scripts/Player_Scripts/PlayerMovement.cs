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
    private float accelerationStatusClamp = 1f;
    private Vector3 totalCollidingDecelerationVector;
    // Statuses
    [SerializeField] private bool isGrounded = false;
    [SerializeField] private bool isSliding = false;

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

    private void Slide (){

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

        accelerationStatus = Mathf.Clamp(accelerationStatus, 0f, accelerationStatusClamp);

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
            // Allow more control of momentum while in mid-air
            Vector3 playerInputInfluence = baseMovementNormalized * airControlSpeed * Time.deltaTime;

            // Combine adjusted velocity, mid-air acceleration, and player input influence
            newVelocity = velocity + playerInputInfluence;
        }

        // Apply clamping to individual components of newVelocity
        newVelocity = new Vector3(
            Mathf.Clamp(newVelocity.x, -maxSpeed, maxSpeed),
            Mathf.Clamp(newVelocity.y, -55f, 55f),
            Mathf.Clamp(newVelocity.z, -maxSpeed, maxSpeed)
        );

        newVelocity -= Vector3.Project(newVelocity, totalCollidingDecelerationVector) / 4f;

        // Update the actual velocity after all calculations
        velocity.x = newVelocity.x;
        velocity.z = newVelocity.z;
    }

    public void CollisionDeceleration (Vector3 displacementVectorNormalized){
        (totalCollidingDecelerationVector += displacementVectorNormalized).Normalize();
        accelerationStatusClamp = .5f;
    }

    public void ClearCollisionDeceleration (Vector3 displacementVectorNormalized, int collisionLeft){
        if (collisionLeft == 0){
            totalCollidingDecelerationVector = Vector3.zero;
            accelerationStatusClamp = 1f;
        } else (totalCollidingDecelerationVector -= displacementVectorNormalized).Normalize();
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
