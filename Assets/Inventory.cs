using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    private HashSet<string> collectedKeys = new HashSet<string>(); // Stores all collected keys

    public void AddKey(string keyName)
    {
        collectedKeys.Add(keyName);
        Debug.Log("Picked up: " + keyName);
    }

    public bool HasKey(string keyName)
    {
        return collectedKeys.Contains(keyName);
    }

    private void Update()
    {
        
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            AddKey("GoldenKey");
            Debug.Log("TEST: You now have the GoldenKey!");
        }
    }
}
