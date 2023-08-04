using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseInput_Handler : MonoBehaviour
{
    [Header ("Events called on ")]
    public Action onLeftMouseButtonPressed;
    public Action onRightMouseButtonPressed;

    private void Update (){
        if (Input.GetMouseButton(0)) onLeftMouseButtonPressed?.Invoke();
        if (Input.GetMouseButton(1)) onRightMouseButtonPressed?.Invoke();
    }
}
