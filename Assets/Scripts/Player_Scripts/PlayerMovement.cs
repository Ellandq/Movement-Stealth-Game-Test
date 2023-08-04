using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header ("Movement Settings")]
    [SerializeField] private float speed = 12f;
    [SerializeField] private float playerGravity = -9.81f;

    [Header ("Player Information")]
    [SerializeField] private Vector3 velocity; // Remove the serialization later
    [SerializeField] private bool isGrounded;

    [Header ("Object References")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private Transform groundCheck;

    [Header ("Misc. Settings")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundDistance = 0.4f;
    
    private void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Player movement 
        // TODO make it so the player adds accelerates rather then move at a constant speed
        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);

        // Gravity
        if (!isGrounded){
            velocity.y += playerGravity * Time.deltaTime;

            controller.Move(velocity * Time.deltaTime);
        }else{
            velocity.y = -2f;
        }
        

        
    }
}
