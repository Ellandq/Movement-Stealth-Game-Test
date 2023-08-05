using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class KeyboardInput_Handler : MonoBehaviour
{
    [Header ("Events called")]
    public Action onJumpButtonPressed;

    [Header ("Keybinds")]
    private KeyCode jumpKey = KeyCode.Space;

    private void Awake (){
        UpdatePlayerKeyBinds();
    }

    private void Update (){
       if (Input.GetKeyDown(jumpKey)) onJumpButtonPressed?.Invoke();
       
    }

    public void UpdatePlayerKeyBinds (){
        //TODO
    }
}
