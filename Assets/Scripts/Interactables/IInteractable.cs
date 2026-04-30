namespace Interactables
{
    /// <summary>
    /// Makes an object be able to be interacted with by the player
    /// </summary>
    public interface IInteractable
    {
        /// <summary>
        /// Checks whether the object can be interacted with
        /// </summary>
        /// <param name="interactor">Which interactor is checking</param>
        /// <returns>bool: true if interactable</returns>
        public bool CanInteract(Interactor interactor);
    
        /// <summary>
        /// Interacts with the object
        /// </summary>
        /// <param name="interactor">Which interactor is using it</param>
        public void Interact(Interactor interactor);
    
        /// <summary>
        /// The initial frame you first meet the interacting conditions
        /// </summary>
        public void StartPreview();
    
        /// <summary>
        /// The frame you stop meeting the interacting conditions
        /// </summary>
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
        /// <param name="interactor">Which interactor to lock to</param>
        public void Lock(Interactor interactor);

        /// <summary>
        /// This unlocks the object from the Interactor, so it can no longer be interacted with if it is out of range
        /// </summary>
        /// <param name="interactor">The Interactor to lock to</param>
        public void Unlock(Interactor interactor);

        /// <summary>
        /// Shows what happens during the interaction
        /// </summary>
        public void ShowInteraction();
    
        /// <summary>
        /// Stops showing what happens during the interaction
        /// </summary>
        public void HideInteraction();
    }
}