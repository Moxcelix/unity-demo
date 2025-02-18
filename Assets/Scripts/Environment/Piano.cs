using Core.Interactive;
using Core.Player;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Piano : MonoBehaviour, IInteractable
{
    [SerializeField] private string _text;
    [SerializeField] private float _range;
    [SerializeField] private Transform _target;
    [SerializeField] private AudioClip _clip;
    [SerializeField] private SeatPlace _seatPlace;

    private AudioSource _source;

    public bool IsAvailable { get; private set; }

    public float Range => _range;
    public string HintText => _text;
    public Transform Target => _target;
    public SeatPlace SeatPlace => _seatPlace;

    private void Awake()
    {
        _source = GetComponent<AudioSource>();

        IsAvailable = true;
    }

    private void Update()
    {
        IsAvailable = !_source.isPlaying;
    }

    public void Interact(IInteractor interactor)
    {
        Debug.Log($"Interacted by {interactor}.");

        interactor.CurrentNPC.CommentInteraction("piano");

        interactor.CurrentNPC.InteractWith(this,
            interactor.CurrentNPC.PlayPiano);
    }

    public void Play()
    {
        _source.PlayOneShot(_clip);
    }
}
