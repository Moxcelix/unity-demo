using UnityEngine;

namespace Core.Interactive
{
    public interface IInteractable
    {
        public GameObject GameObject { get; }
        public string HintText { get; } 
        public bool IsAvailable { get; }   
        public void Interact(IInteractor interactor);
    }
}
