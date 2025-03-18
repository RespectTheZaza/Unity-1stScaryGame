using UnityEngine;
using FishNet.Object;

public class KeyPickup : NetworkBehaviour, IInteractable
{
    [SerializeField] private string keyName = "GoldenKey";

    public string InteractionPrompt => "Pick up " + keyName;

    public bool Interact(Interactor interactor)
    {
        if (!IsServer) // Ensure only the server handles item pickup
        {
            RequestPickupServerRpc(interactor.GetComponent<NetworkObject>());
            return false;
        }

        PickupKey(interactor.GetComponent<NetworkObject>());
        return true;
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestPickupServerRpc(NetworkObject playerObject)
    {
        if (playerObject != null)
        {
            PickupKey(playerObject);
        }
    }

    private void PickupKey(NetworkObject playerObject)
    {
        Inventory inventory = playerObject.GetComponent<Inventory>();
        if (inventory != null)
        {
            inventory.AddKey(keyName);
            RemoveKeyFromSceneObserversRpc();
        }
    }

    [ObserversRpc]
    private void RemoveKeyFromSceneObserversRpc()
    {
        gameObject.SetActive(false); // Hide key for all players
    }
}
