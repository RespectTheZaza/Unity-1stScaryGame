using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections; 
using System.Collections.Generic;
using System;

public class Inventory : MonoBehaviour
{
    public bool HasKey = false; 

    private void Update()
    {
        if(Keyboard.current.qKey.wasPressedThisFrame) HasKey = !HasKey;
    }
}
