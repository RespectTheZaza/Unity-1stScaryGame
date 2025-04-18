using UnityEngine;

public class Chest : MonoBehaviour, IInteractable 
{
    [SerializeField] private string _prompt;

    public string InteractionPrompt => _prompt;
    
    public bool Interact(Interactor interactor)
    {
        Debug.Log("Opening chest");
        return true;
    }
}
