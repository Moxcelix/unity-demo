using Core.Interactive;
using UnityEngine;

public class Piano : MonoBehaviour, IInteractable
{
    [SerializeField] private string _text;

    public string HintText => _text;
    public bool IsAvailable => true;

    public void Interact()
    {
        Debug.Log("Interacted");
    }
}
