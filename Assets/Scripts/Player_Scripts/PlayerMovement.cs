using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header ("Movement Settings")]
    [SerializeField] private float speed = 12f;
    [SerializeField] private float playerGravity = -9.81f;
    [SerializeField] private float jumpHeight = 3f;

    [Header ("Player Information")]
    [SerializeField] private Vector3 velocity; // Remove the serialization later
    [SerializeField] private bool isGrounded;

    [Header ("Object References")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private Transform groundCheck;

    [Header ("Misc. Settings")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundDistance = 0.4f;

    [Header ("Jump Settings")]
    [SerializeField] private float jumpPower;
    

    

    private void Start (){

    }

    private void Update()
    {
        UpdateGroundedStatus();

        GravityAcceleration();

        UserMovement();
    }

    #region Player Checks

    private void UpdateGroundedStatus (){
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }

    #endregion

    #region Player Movement

    private void Jump (){
        if (!isGrounded) return;

        velocity.y = Mathf.Sqrt(jumpHeight * -2f * playerGravity);
    }

    private void UserMovement (){
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);
    }

    private void GravityAcceleration (){
        if (!isGrounded){
            velocity.y += playerGravity * Time.deltaTime;

            controller.Move(velocity * Time.deltaTime);
        }else{
            velocity.y = -2f;
        }
    }
    
    #endregion

    #region Getters

    public bool IsGrounded(){
        return isGrounded;
    }

    #endregion
}
