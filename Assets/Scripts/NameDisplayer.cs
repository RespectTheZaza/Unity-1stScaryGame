using UnityEngine;
using TMPro;
using FishNet.Object;
using FishNet.Connection;
using System;

public class NameDisplayer : NetworkBehaviour
{
    [SerializeField]
    private TextMeshPro _text;

public override void OnStartClient() {
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
        if (conn != base.Owner)
            return;

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
}
