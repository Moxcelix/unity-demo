using Core.Interactive;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BagirovPainting : MonoBehaviour, IInteractable
{
    [SerializeField] private string _hint;
    [SerializeField] private float _range;
    [SerializeField] private Transform _viewPoint;

    [SerializeField] private AudioClip _rusy;

    private AudioSource _audioSource;

    public Transform Target => _viewPoint;
    public float Range => _range;
    public string HintText => _hint;
    public bool IsAvailable => !_audioSource.isPlaying;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void Interact(IInteractor interactor)
    {
        Debug.Log($"Interacted by {interactor}.");

        interactor.CurrentNPC.CommentInteraction("painting");

        interactor.CurrentNPC.InteractWith(this, () =>
            {
                interactor.CurrentNPC.SetLookTarget(transform);
                _audioSource.PlayOneShot(_rusy);
            });
    }
}
