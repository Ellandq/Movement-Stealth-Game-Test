using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    // Static instance of the Inputa Manager class
    public static InputManager Instance;

    [Header ("Object References")]
    [SerializeField] private KeyboardInput_Handler keyboardInput_Handler;
    [SerializeField] private MouseInput_Handler mouseInput_Handler;

    private void Awake()
    {
        // Instance initialization
        Instance = this;
    }

    // Getters

    public KeyboardInput_Handler GetKeyboardInputHandler (){
        return keyboardInput_Handler;
    }

    public MouseInput_Handler GetMouseInputHandler (){
        return mouseInput_Handler;
    }


}
