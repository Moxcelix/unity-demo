using UnityEngine;

namespace Core.Interactive
{
    public interface IInteractable
    {
        public Transform Target { get; }
        public float Range { get; }
        public string HintText { get; } 
        public bool IsAvailable { get; }   
        public void Interact(NPC interactor);
    }
}
