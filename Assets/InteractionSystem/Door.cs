using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private string _prompt = "Open Door"; // Interaction text
    [SerializeField] private string requiredKey = "GoldenKey"; // Set in Inspector
    [SerializeField] private Transform doorTransform; // Assign the door object in Inspector
    private bool isOpen = false;
    private float openAngle = 90f; // Adjust as needed

    public string InteractionPrompt => _prompt;

    public bool Interact(Interactor interactor)
    {
        var inventory = interactor.GetComponent<Inventory>();

        if (inventory == null) return false;

        if (inventory.HasKey(requiredKey)) // Check if player has the right key
        {
            ToggleDoor();
            return true;
        }

        Debug.Log("No key found! You need: " + requiredKey);
        return false;
    }

    private void ToggleDoor()
    {
        isOpen = !isOpen;
        float targetRotation = isOpen ? openAngle : 0f;
        doorTransform.localRotation = Quaternion.Euler(0, targetRotation, 0);
        Debug.Log(isOpen ? "Door opened!" : "Door closed!");
    }
}
