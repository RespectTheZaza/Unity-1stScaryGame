using UnityEngine;
using System.Collections;
using FishNet.Object;
using FishNet.Connection;
using FishNet.Managing;

public class Door : NetworkBehaviour, IInteractable
{
    [Header("Door Settings")]
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

    private void Start()
    {
        Debug.Log($"Door Spawned {base.IsSpawned} | IsServer? {base.IsServer} | IsClient? {base.IsClient}");
    }

    public bool Interact(Interactor interactor)
    {
        Debug.Log("Interact() called on Door!");

        if (!base.IsClient || base.NetworkManager == null || !base.NetworkManager.IsClientStarted)
        {
            Debug.LogWarning("Client is not active or not connected to the server, Cannot send ServerRpc.");
            return false;
        }

        Debug.Log("Not the server, sending ServerRpc to request door toggle");
        RequestDoorToggleServerRpc(interactor.GetComponent<NetworkObject>().Owner.ClientId);
        return false;
    }

    [ServerRpc(RequireOwnership = false)] 
    private void RequestDoorToggleServerRpc(int clientId)
    {
        Debug.Log($"Server received door toggle request from client: {clientId}");

        if (base.NetworkManager == null || !base.NetworkManager.ServerManager.Clients.ContainsKey(clientId))
        {
            Debug.LogError("Client not found or NetworkManager is missing.");
            return;
        }

        NetworkObject playerObject = base.NetworkManager.ServerManager.Clients[clientId].FirstObject;
        Inventory inventory = playerObject.GetComponent<Inventory>();

        if (inventory != null && inventory.HasKey(requiredKey))
        {
            Debug.Log("Player has the required key. Toggling door");
            ToggleDoorServerRpc();
        }
        else
        {
            Debug.Log("Player does NOT have the required key.");
        }
    }

    [ServerRpc(RequireOwnership = false)] // Allow all players to open the door
    private void ToggleDoorServerRpc()
    {
        Debug.Log("ServerRpc: Toggling door...");
        isOpen = !isOpen;
        ToggleDoorObserversRpc(isOpen);
    }

    [ObserversRpc] // Ensures door opens for all players
    private void ToggleDoorObserversRpc(bool state)
    {
        Debug.Log($"ObserversRpc: Door is now {(state ? "OPEN" : "CLOSED")}");

        if (doorCoroutine != null) StopCoroutine(doorCoroutine);
        doorCoroutine = StartCoroutine(ToggleDoorAnimation(state));
    }

    private IEnumerator ToggleDoorAnimation(bool opening)
    {
        Debug.Log("Animating door...");
        float targetAngle = opening ? openAngle : closeAngle;
        float startAngle = doorTransform.localRotation.eulerAngles.y;
        float elapsedTime = 0f;

        if (doorSound != null) doorSound.Play();

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * openSpeed;
            float newAngle = Mathf.LerpAngle(startAngle, targetAngle, elapsedTime);
            doorTransform.localRotation = Quaternion.Euler(0, newAngle, 0);
            yield return null;
        }

        doorTransform.localRotation = Quaternion.Euler(0, targetAngle, 0);
        Debug.Log("Door animation completed.");
    }
}
