using Interactables;
using UnityEngine;
using UnityEngine.Events;

public interface IInteractable
{
    public bool CanInteract(Interactor interactor);
    public void Interact(Interactor interactor);
    public void StartPreview();
    public void StopPreview();

    /// <summary>
    /// Returns whether the object can be locked to the Interactor. An object locked to an Interactor can be interacted even if not in interaction range
    /// </summary>
    /// <returns>true|false</returns>
    public bool IsLockable();

    /// <summary>
    /// Whether the object is locked to an Interactor already
    /// </summary>
    /// <returns>true if locked, false if not locked</returns>
    public bool IsLocked();

    /// <summary>
    /// This locks the object to the Interactor so that it can be interacted with even if it is not in range. Remember to call Unlock from the interactable object as applicable
    /// </summary>
    /// <param name="interactor"></param>
    public void Lock(Interactor interactor);

    /// <summary>
    /// This unlocks the object from the Interactor, so it can no longer be interacted with if it is out of range
    /// </summary>
    /// <param name="interactor">The Interactor to lock to</param>
    public void Unlock(Interactor interactor);
}