namespace Core.Interactive
{
    public interface IInteractable
    {
        public string HintText { get; }
        public bool IsAvailable { get; }   
        public void Interact();
    }
}
