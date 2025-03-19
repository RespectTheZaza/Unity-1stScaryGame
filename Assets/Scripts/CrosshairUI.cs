using UnityEngine;
using UnityEngine.UI;

public class CrosshairUI : MonoBehaviour
{
    [SerializeField] private Image crosshairImage; // Assign in the Inspector
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color interactColor = Color.red;
    
    public static CrosshairUI Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void SetCrosshairColor(bool isLookingAtInteractable)
    {
        crosshairImage.color = isLookingAtInteractable ? interactColor : defaultColor;
    }
}
