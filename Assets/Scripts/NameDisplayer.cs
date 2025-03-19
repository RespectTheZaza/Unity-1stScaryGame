using UnityEngine;
using TMPro;
using FishNet.Object;
using FishNet.Connection;

public class NameDisplayer : NetworkBehaviour
{
    [SerializeField] private TextMeshPro _text;
    [SerializeField] private Transform playerTransform; // Assign the player's root object
    [SerializeField] private Transform nameTagTransform; // Assign the NameTag (TextMeshPro Object)

    public override void OnStartClient()
    {
        base.OnStartClient();
        PlayerNameTracker.OnNameChange += PlayerNameTracker_OnNameChange;
        SetName();
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        PlayerNameTracker.OnNameChange -= PlayerNameTracker_OnNameChange;
    }

    public override void OnOwnershipClient(NetworkConnection prevOwner)
    {
        base.OnOwnershipClient(prevOwner);
        SetName();
    }

    private void PlayerNameTracker_OnNameChange(NetworkConnection conn, string newName)
    {
        if (conn != base.Owner) return;
        SetName();
    }

    private void SetName()
    {
        string result = null;

        if (base.Owner.IsValid)
            result = PlayerNameTracker.GetPlayerName(base.Owner);

        if (string.IsNullOrEmpty(result))
            result = "Patient";

        _text.text = result;
    }

    private void LateUpdate()
    {
        if (Camera.main == null || nameTagTransform == null) return;

        // ✅ Rotate name tag to always face the camera
        nameTagTransform.LookAt(Camera.main.transform);
        nameTagTransform.Rotate(0, 180, 0); // Flip to avoid mirroring

        // ✅ Keep the name tag above the player
        if (playerTransform != null)
            nameTagTransform.position = playerTransform.position + Vector3.up * 2f;
    }
}
