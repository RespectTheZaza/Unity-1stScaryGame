using UnityEngine;
using System.Collections;
using FishNet.Object; // FishNet networking

public class Door : NetworkBehaviour, IInteractable
{
    [SerializeField] private string _prompt = "Open Door";
    [SerializeField] private string requiredKey = "GoldenKey";
    [SerializeField] private Transform doorTransform;
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float closeAngle = 0f;
    [SerializeField] private float openSpeed = 2f;
    [SerializeField] private AudioSource doorSound;

    private bool isOpen = false;
    private Coroutine doorCoroutine;

    public string InteractionPrompt => _prompt;

    public bool Interact(Interactor interactor)
    {
        if (!IsServer) // Ensure only the server controls the door state
        {
            RequestDoorToggleServerRpc();
            return false;
        }

        var inventory = interactor.GetComponent<Inventory>();

        if (inventory != null && inventory.HasKey(requiredKey))
        {
            ToggleDoorServerRpc();
            return true;
        }

        Debug.Log("No key found! You need: " + requiredKey);
        return false;
    }

    [ServerRpc(RequireOwnership = false)] // Allows any player to request a door toggle
    private void ToggleDoorServerRpc()
    {
        isOpen = !isOpen;
        ToggleDoorObserversRpc(isOpen);
    }

    [ObserversRpc] // Syncs door opening for all players
    private void ToggleDoorObserversRpc(bool state)
    {
        if (doorCoroutine != null) StopCoroutine(doorCoroutine);
        doorCoroutine = StartCoroutine(ToggleDoorAnimation(state));
    }

    private IEnumerator ToggleDoorAnimation(bool opening)
    {
        float targetAngle = opening ? openAngle : closeAngle;
        float startAngle = doorTransform.localRotation.eulerAngles.y;
        float elapsedTime = 0f;

        if (doorSound != null) doorSound.Play();

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * openSpeed;
            float newAngle = Mathf.Lerp(startAngle, targetAngle, elapsedTime);
            doorTransform.localRotation = Quaternion.Euler(0, newAngle, 0);
            yield return null;
        }

        doorTransform.localRotation = Quaternion.Euler(0, targetAngle, 0);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestDoorToggleServerRpc()
    {
        ToggleDoorServerRpc();
    }
}
