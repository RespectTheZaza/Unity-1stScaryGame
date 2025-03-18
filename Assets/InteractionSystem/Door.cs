using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private string _prompt;

    public string InteractionPrompt => _prompt;
    
    public bool Interact(Interactor interactor)
    {

        var Inventory = interactor.GetComponent<Inventory>();

        if (Inventory == null) return false;

        if (Inventory.HasKey)
        {
            Debug.Log("opening door");
            return true;
        }

        Debug.Log("no key found");
        return false;
    }

}
