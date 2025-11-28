using Interactables;
using UnityEngine;

public interface IInteractable
{
    public bool CanInteract();
    public void Interact(Interactor interactor);
    public void StartPreview();
    public void StopPreview();
}
