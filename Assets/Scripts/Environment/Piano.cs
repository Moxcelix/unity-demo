using Core.Interactive;
using UnityEngine;

public class Piano : MonoBehaviour, IInteractable
{
    [SerializeField] private string _text;

    public string HintText => _text;
    public bool IsAvailable => true;
    public GameObject GameObject => gameObject;

    public void Interact(IInteractor interactor)
    {
        Debug.Log($"Interacted by {interactor}.");
    }
}
