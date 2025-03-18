using UnityEngine;

public class KeyPickup : MonoBehaviour, IInteractable
{
    [SerializeField] private string keyName = "GoldenKey"; // Set in Inspector

    public string InteractionPrompt => "Pick up " + keyName;

    public bool Interact(Interactor interactor)
    {
        var inventory = interactor.GetComponent<Inventory>();

        if (inventory != null)
        {
            inventory.AddKey(keyName);
            Destroy(gameObject); // Remove key from scene
            return true;
        }

        return false;
    }
}
