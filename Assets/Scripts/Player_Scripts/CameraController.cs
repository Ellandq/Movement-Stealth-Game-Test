using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header ("Object References")]
    [SerializeField] private Transform playerBody;

    [Header ("Camera Settings")]
    [SerializeField] private float mouseSensitivity = 100f;


    private bool playerCameraStatus = false;
    private float xRotation = 0f;

    // Start is called before the first frame update
    private void Start()
    {   
        // This should be called outise the script when activating the player character

        ChangePlayerCameraState();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!playerCameraStatus) return;
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }

    public void ChangePlayerCameraState (){
        if (playerCameraStatus){
            Cursor.lockState = CursorLockMode.Locked;
        }else{
            Cursor.lockState = CursorLockMode.None;
        }
        playerCameraStatus = !playerCameraStatus;
    }
}
