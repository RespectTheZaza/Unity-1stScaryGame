using UnityEngine;
using FishNet.Object;

public class FPSCamera : NetworkBehaviour
{
    public float mouseSensitivity = 150f;
    public Transform playerBody;
    public Camera playerCamera; // Reference this directly in the Inspector

    private float xRotation = 0f;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsOwner)
        {
            gameObject.SetActive(true);
            LockCursor(); // Lock cursor when the player spawns
        }
    }

    void Update()
    {
        if (!base.IsOwner) return;

        HandleMouseLock(); // Check if player presses Escape

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked; // Locks cursor to the center of the screen
        Cursor.visible = false; // Hides the cursor
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None; // Unlocks cursor
        Cursor.visible = true; // Makes cursor visible again
    }

    private void HandleMouseLock()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                UnlockCursor(); // Unlock the cursor when Escape is pressed
            }
            else
            {
                LockCursor(); // Relock the cursor when Escape is pressed again
            }
        }
    }
}
