using UnityEngine;
using FishNet.Object;
using UnityEngine.InputSystem;

public class Flashlight : NetworkBehaviour
{
    [SerializeField] private Light flashlight; 
    [SerializeField] private Transform cameraTransform; 
    [SerializeField] private Transform flashlightPivot; 
    [SerializeField] private GameObject lightObject; 

    public float flashlightSmoothSpeed = 10f;
    public float maxVerticalAngle = 60f;
    public float minVerticalAngle = -60f;

    private bool isOn = false; 

    void Awake()
    {
        
        if (flashlightPivot != null)
        {
            flashlightPivot.gameObject.SetActive(true);
        }
    }

public override void OnStartClient()
{
    base.OnStartClient();

    if (flashlightPivot == null)
    {
        Debug.LogError("FlashlightPivot is not assigned! Assign it in the Inspector.");
        return;
    }

    
    flashlightPivot.gameObject.SetActive(true);
    Debug.Log("FlashlightPivot forced active on client start.");

    if (!IsOwner) return; 

    
    SetFlashlightState(false);
}


    void Update()
    {
        if (!IsOwner) return;

        
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            isOn = !isOn;
            ToggleFlashlightServerRpc(isOn);
        }

        AlignFlashlightWithView();
    }

    private void AlignFlashlightWithView()
    {
        if (flashlightPivot == null || cameraTransform == null) return;

        flashlightPivot.rotation = cameraTransform.rotation;

        Vector3 currentRotation = flashlightPivot.localEulerAngles;
        if (currentRotation.x > 180f) currentRotation.x -= 360f;
        currentRotation.x = Mathf.Clamp(currentRotation.x, minVerticalAngle, maxVerticalAngle);

        flashlightPivot.localRotation = Quaternion.Euler(currentRotation);

        
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
        SetFlashlightState(state);
    }

    private void SetFlashlightState(bool state)
    {
        if (flashlight == null)
        {
            Debug.LogError("Flashlight is not assigned!");
            return;
        }

        flashlight.enabled = state;
        if (lightObject != null) lightObject.SetActive(state); 
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
