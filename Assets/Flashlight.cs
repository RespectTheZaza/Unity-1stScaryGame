using UnityEngine;
using FishNet.Object;
using UnityEngine.InputSystem;

public class Flashlight : NetworkBehaviour
{
    [SerializeField] private Light flashlight; // Assign in Inspector
    [SerializeField] private Transform cameraTransform; // Assign Player Camera in Inspector
    [SerializeField] private Transform flashlightPivot; // Empty GameObject used to rotate flashlight

    public float flashlightSmoothSpeed = 10f;
    public float maxVerticalAngle = 60f;
    public float minVerticalAngle = -60f;

    private bool isOn = false; // Local flashlight state

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!IsOwner) return;

        flashlight.enabled = false; // Ensure flashlight starts OFF
    }

    void Update()
    {
        if (!IsOwner) return;

        // Toggle Flashlight ON/OFF with "F"
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            isOn = !isOn;
            ToggleFlashlightServerRpc(isOn);
        }

        AlignFlashlightWithView();
    }

    private void AlignFlashlightWithView()
    {
        flashlightPivot.rotation = cameraTransform.rotation;

        Vector3 currentRotation = flashlightPivot.localEulerAngles;
        if (currentRotation.x > 180f) currentRotation.x -= 360f;
        currentRotation.x = Mathf.Clamp(currentRotation.x, minVerticalAngle, maxVerticalAngle);

        flashlightPivot.localRotation = Quaternion.Euler(currentRotation);

        // ✅ Sync flashlight rotation
        UpdateFlashlightRotationServerRpc(flashlightPivot.rotation);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ToggleFlashlightServerRpc(bool state)
    {
        isOn = state;
        ToggleFlashlightObserversRpc(state);
    }

    [ObserversRpc(BufferLast = true)]
    private void ToggleFlashlightObserversRpc(bool state)
    {
        flashlight.enabled = state;
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdateFlashlightRotationServerRpc(Quaternion newRotation)
    {
        UpdateFlashlightRotationObserversRpc(newRotation);
    }

    [ObserversRpc(BufferLast = true)]
    private void UpdateFlashlightRotationObserversRpc(Quaternion newRotation)
    {
        flashlightPivot.rotation = Quaternion.Slerp(flashlightPivot.rotation, newRotation, flashlightSmoothSpeed * Time.deltaTime);
    }
}
