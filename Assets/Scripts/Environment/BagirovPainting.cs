using Core.Interactive;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BagirovPainting : MonoBehaviour, INPCInteractable
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

    public void Interact(NPC interactor)
    {
        interactor.SetLookTarget(transform);

        _audioSource.PlayOneShot(_rusy);
    }
}
