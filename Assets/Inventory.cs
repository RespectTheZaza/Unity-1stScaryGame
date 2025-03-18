using UnityEngine;
using FishNet.Object;
using System.Collections.Generic;

public class Inventory : NetworkBehaviour
{
    private HashSet<string> collectedKeys = new HashSet<string>();

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!IsOwner) return;

        Debug.Log("Inventory initialized for player: " + Owner.ClientId);
    }

    public void AddKey(string keyName)
    {
        if (IsServer)
        {
            collectedKeys.Add(keyName);
            UpdateKeyCountObserversRpc(collectedKeys.Count);
        }
        else
        {
            RequestAddKeyServerRpc(keyName);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestAddKeyServerRpc(string keyName)
    {
        collectedKeys.Add(keyName);
        UpdateKeyCountObserversRpc(collectedKeys.Count);
    }

    [ObserversRpc]
    private void UpdateKeyCountObserversRpc(int newCount)
    {
        Debug.Log($"Key count updated for all players: {newCount}");
    }

    public bool HasKey(string keyName)
    {
        return collectedKeys.Contains(keyName);
    }
}
