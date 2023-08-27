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
    [SerializeField] private float acceleration = .5f;
    [SerializeField] private float deceleration = .4f;
    [SerializeField] private float slidingDeceleration;
    [SerializeField] private float maxHorizontalSlidingSpeed;
    // Vertical movement
    [SerializeField] private float jumpHeight = 4f;
    [SerializeField] private float gravityMultiplier = 1f;
    // Slope settings
    [SerializeField] private float maxSlopeAngle = 75f;

    [Header ("Player Information")]
    // Movement
    [SerializeField] private float currentVelocity;
    private Vector3 velocity;
    private float accelerationStatus = 0f;
    private float accelerationStatusClamp = 1f;
    private Vector3 totalCollidingDecelerationVector;
    private Vector3 lastSavedSlidingDirection;
    private Vector3 currentSlidingHorizontalMovement;
    // Size
    private float startYScale;
    private float reducedYScale = .5f;
    // Statuses
    [SerializeField] private bool isGrounded = false;
    [SerializeField] private bool isSliding = false;

    [Header ("Object References")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private Transform groundCheck;

    [Header ("Misc. Settings")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundDistance = .4f;

    private void Start (){
        startYScale = transform.localScale.y;
        currentSlidingHorizontalMovement = Vector3.zero;

        InputManager.Instance.GetKeyboardInputHandler().onJumpButtonPressed += Jump;
        InputManager.Instance.GetKeyboardInputHandler().onSlideButtonDown += EnableSlide;
        InputManager.Instance.GetKeyboardInputHandler().onSlideButtonUp += DisableSlide;
    }

    private void Update()
    {
        UpdateGroundedStatus();

        GravityAcceleration();

        if (!isSliding) UserMovement();
        else Slide();

        controller.Move(velocity * Time.deltaTime);

        currentVelocity = new Vector3(velocity.x, 0f, velocity.z).magnitude;

        if (currentVelocity == 0f) accelerationStatus = 0f;
    }

    #region Player States

    private void UpdateGroundedStatus (){
        isGrounded = controller.isGrounded || Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }

    private void EnableSlide (){
        if (!isGrounded && currentVelocity > 0f) return;
        // Position adjustment
        Vector3 adjustedPosition = transform.position;
        adjustedPosition.y -= .95f;
        transform.position = adjustedPosition;

        currentSlidingHorizontalMovement = Vector3.zero;

        // Scale adjustment
        transform.localScale = new Vector3(1f, reducedYScale, 1f);

        isSliding = true;
        lastSavedSlidingDirection = new Vector3(velocity.x, 0f, velocity.z).normalized;
        gravityMultiplier = 4f;
    }

    private void DisableSlide (){
        transform.localScale = new Vector3(1f, startYScale, 1f);
        isSliding = false;
        gravityMultiplier = 1f;
    }

    #endregion

    #region Player Movement

    private void Jump (){
        if (!isGrounded) return;
        DisableSlide();
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);
    }

    private void Slide (){
        float x = Input.GetAxisRaw("Horizontal");

        Vector3 horizontalVelocity = new Vector3(velocity.x, 0f, velocity.z);
        Vector3 newVelocity = horizontalVelocity - (lastSavedSlidingDirection * slidingDeceleration
        + GetCurrentSlopeDownwardsVector() * Physics.gravity.magnitude) * Time.deltaTime;

        if (newVelocity.magnitude < 5f || Vector3.Dot(lastSavedSlidingDirection, newVelocity.normalized) < 0.7f){
            DisableSlide();
            return;
        }
        
        // Side movement
        currentSlidingHorizontalMovement += transform.right * x * speed * Time.deltaTime;

        if (currentSlidingHorizontalMovement.magnitude < maxHorizontalSlidingSpeed){
            newVelocity += transform.right * x * speed * Time.deltaTime;
        }

        // Collision deceleration
        newVelocity -= Vector3.Project(newVelocity, totalCollidingDecelerationVector) / 2f;

        // Update the actual velocity after all calculations
        velocity.x = newVelocity.x;
        velocity.z = newVelocity.z;
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

        if (isGrounded) newVelocity -= Vector3.Project(newVelocity, totalCollidingDecelerationVector) / 2f;

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
            velocity.y += Physics.gravity.y * Time.deltaTime * gravityMultiplier;
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

    public bool IsSliding (){
        return isSliding;
    }

    public float GetCurrentVelocity(){
        return currentVelocity;
    }

    public Vector3 GetCurrentSlopeDownwardsVector (){
        Ray ray = new Ray(transform.position + Vector3.up * 0.1f, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 2f, groundMask))
        {
            float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);

            if (slopeAngle <= maxSlopeAngle)
            {
                // Calculate the normalized directional vector of the slope
                Vector3 slopeDirection = -Vector3.Cross(Vector3.Cross(Vector3.up, hit.normal), hit.normal).normalized;
                slopeDirection.y = -slopeDirection.y;
                Debug.Log(slopeDirection);

                return slopeDirection;
            }
        }
        return Vector3.zero;
    }

    #endregion
}
