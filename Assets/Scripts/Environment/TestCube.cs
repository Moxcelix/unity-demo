using UnityEngine;
using Core.Interactive;

public class TestCube : MonoBehaviour, IInteractable
{
    [SerializeField] private string _text;

    public string HintText => _text;
    public bool IsAvailable => true;

    public void Interact()
    {
        Debug.Log("Cube interacted");
    }
}
