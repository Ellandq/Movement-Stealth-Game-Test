using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class KeyboardInput_Handler : MonoBehaviour
{
    [Header ("Events called")]
    public Action onJumpButtonPressed;
    public Action onSlideButtonDown;
    public Action onSlideButtonUp;

    [Header ("Keybinds")]
    private KeyCode jumpKey = KeyCode.Space;
    private KeyCode slideKey = KeyCode.LeftControl;

    private void Awake (){
        UpdatePlayerKeyBinds();
    }

    private void Update (){
        // Jump
        if (Input.GetKeyDown(jumpKey)) onJumpButtonPressed?.Invoke();
        // Slide
        if (Input.GetKeyDown(slideKey)) onSlideButtonDown?.Invoke();
        else if (Input.GetKeyUp(slideKey)) onSlideButtonUp?.Invoke();
    }

    public void UpdatePlayerKeyBinds (){
        //TODO
    }
}
