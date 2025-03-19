using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using FishNet.Object;

public class Interactor : NetworkBehaviour
{
    [SerializeField] private Transform _interactionPoint;
    [SerializeField] private float _interactionPointRadius = 0.5f;
    [SerializeField] private LayerMask _interactionMask;

    private readonly Collider[] _colliders = new Collider[3];
    [SerializeField] private int _numFound;

    private void Update()
    {
        _numFound = Physics.OverlapSphereNonAlloc(
            _interactionPoint.position, _interactionPointRadius, 
            _colliders, _interactionMask);

        bool isLookingAtInteractable = false;

        if (_numFound > 0)
        {
            var interactable = _colliders[0].GetComponent<IInteractable>();

            if (interactable != null)
            {
                isLookingAtInteractable = true;

                if (Keyboard.current.eKey.wasPressedThisFrame)
                {
                    Debug.Log("Pressed 'E' - Attempting to interact with " + _colliders[0].gameObject.name);
                    interactable.Interact(this);
                }
            }
        }

        // Update Crosshair UI
        if (CrosshairUI.Instance != null)
        {
            CrosshairUI.Instance.SetCrosshairColor(isLookingAtInteractable);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_interactionPoint.position, _interactionPointRadius);
    }
}
