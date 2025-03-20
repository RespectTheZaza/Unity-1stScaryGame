using UnityEngine;
using FishNet.Object;

public class FPSCamera : NetworkBehaviour
{
    public float mouseSensitivity = 150f;
    public Transform playerBody;
    public Camera playerCamera; 

    private float xRotation = 0f;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsOwner)
        {
            gameObject.SetActive(true);
            LockCursor(); 
        }
    }

    void Update()
    {
        if (!base.IsOwner) return;

        HandleMouseLock(); 

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked; 
        Cursor.visible = false; 
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true; 
    }

    private void HandleMouseLock()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                UnlockCursor(); 
            }
            else
            {
                LockCursor(); 
            }
        }
    }
}
