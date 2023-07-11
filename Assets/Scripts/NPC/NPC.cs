using Core.Interactive;
using Core.Player;
using System;
using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Animator))]
public class NPC : MonoBehaviour, IInteractable, ISitable
{
    private const float c_translateTolerance = 0.05f;

    [SerializeField] private string _hint;
    [SerializeField] private float _rotatinonSpeed;
    [SerializeField] private float _translateSpeed;

    [Header("NPC Sound")]
    [SerializeField] private NPCReplica[] _replicas;

    [Header("NPC States")]
    [SerializeField] private Idling _idlingState;
    [SerializeField] private Greeting _greetingState;
    [SerializeField] private Walking _walkingState;
    [SerializeField] private PlayingPiano _playingPianoState;
    [SerializeField] private Looking _lookingState;

    private Transform _playerTransform;
    private Transform _lookAtTarget;
    private NavMeshAgent _navMeshAgent;
    private AudioSource _audioSource;
    private Animator _animator;
    private NPCState _state;

    public Transform Target => transform;
    public string HintText => _hint;
    public float Range => 0.0f;
    public bool IsAvailable { get; private set; }
    public bool IsSitting { get; private set; }

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _audioSource = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();

        _state = _idlingState;

        _idlingState.Initialize(this);
        _greetingState.Initialize(this);
        _walkingState.Initialize(this);
        _playingPianoState.Initialize(this);
        _lookingState.Initialize(this);

        _idlingState.Start();
    }

    private void Update()
    {
        IsAvailable = _state.IsCompleted && !IsSitting;

        if (_state == _idlingState)
        {
            if (_playerTransform != null &&
                Vector3.Distance(transform.position,
                _playerTransform.position) > 5.0f)
            {
                Follow(_playerTransform, 2.0f);
            }
        }
        else if (_state.IsCompleted)
        {
            SetState(_idlingState);
        }

        if (_state == _playingPianoState)
        {
            if (_playingPianoState.Piano.IsAvailable)
            {
                _playingPianoState.Piano.SeatPlace.Free();
            }
            else
            {
                _playingPianoState.Piano.SeatPlace.Take(this);
            }
        }

        if (_state.LookAtPlayer)
        {
            _lookAtTarget = _playerTransform;
        }

        if (_lookAtTarget != null)
        {
            LookAt(_lookAtTarget.position, Time.deltaTime);
        }
    }

    public void CommentInteraction(string key)
    {
        if (!IsAvailable)
        {
            return;
        }

        Say(key);
    }

    public void Say(string key)
    {
        var replica = from r in _replicas
                      where r.Name == key
                      select r;

        if (replica is null)
        {
            return;
        }

        _audioSource.PlayOneShot(
            replica.First().GetRandomAudioClip());
    }

    public void SetLookTarget(Transform target)
    {
        _lookingState.LookTarget = target;

        SetState(_lookingState);
    }

    public void PlayPiano(Piano piano)
    {
        _playingPianoState.Piano = piano;

        SetState(_playingPianoState);

        piano.Play();
    }

    public void InteractWith<T>(T interactable,
        Action action = null) where T : IInteractable
    {
        if (!IsAvailable)
        {
            return;
        }

        StartCoroutine(GoToAndDo(
            interactable.Target.position,
            interactable.Range, action));
    }

    public void InteractWith<T>(T interactable,
        Action<T> action = null) where T : IInteractable
    {
        InteractWith(interactable,
            () => action(interactable));
    }

    public void Interact(IInteractor interactor)
    {
        _navMeshAgent.ResetPath();
        _playerTransform = interactor.GameObject.transform;

        Say("hi");
        SetState(_greetingState);
    }

    public void PlayAnimation(string arg)
    {
        _animator.SetTrigger(arg);
    }

    public void SitDown(Transform placePoint)
    {
        _navMeshAgent.ResetPath();

        _navMeshAgent.enabled = false;

        IsSitting = true;

        StartCoroutine(TranslateAndDo(
            placePoint.position, c_translateTolerance));
    }

    public void StandUp(Transform leavePoint)
    {
        StartCoroutine(TranslateAndDo(leavePoint.position,
            c_translateTolerance, () =>
            {
                IsSitting = false;
                _navMeshAgent.enabled = true;
            }));

        SetState(_idlingState);
    }

    public void FollowTarget(Transform transform)
    {
        if (transform == null)
        {
            throw new NullReferenceException();
        }

        _lookAtTarget = transform;
    }

    public void ClearFollowTarget()
    {
        _lookAtTarget = null;
    }

    public void StayStill()
    {
        _navMeshAgent.ResetPath();
    }

    private void SetState(NPCState state)
    {
        _state = state;

        ClearFollowTarget();

        _state.Start();
    }

    private IEnumerator TranslateAndDo(Vector3 position,
        float tolerance, Action onEndOfTranslation = null)
    {
        var distance = Vector3.Distance(position, transform.position);

        while (distance > tolerance)
        {
            distance = Vector3.Distance(position, transform.position);

            transform.position = Vector3.Lerp(transform.position,
                position, Time.deltaTime * _translateSpeed);

            yield return new WaitForEndOfFrame();
        }

        transform.position = position;

        onEndOfTranslation?.Invoke();
    }

    private IEnumerator GoToAndDo(Vector3 position,
        float tolerance, Action onEndOfPath = null)
    {
        _navMeshAgent.SetDestination(position);

        var distance = Vector3.Distance(position, transform.position);

        if (distance > tolerance)
        {
            SetState(_walkingState);
        }

        while (distance > tolerance)
        {
            distance = Vector3.Distance(position, transform.position);

            yield return new WaitForEndOfFrame();
        }

        _navMeshAgent.ResetPath();

        onEndOfPath?.Invoke();
    }

    private void LookAt(Vector3 position, float deltaTime)
    {
        var target = new Vector3(position.x,
                transform.position.y, position.z);

        var targetRotation = Quaternion.LookRotation(
            target - transform.position);

        transform.rotation = Quaternion.Slerp(
            transform.rotation, targetRotation,
            _rotatinonSpeed * deltaTime);
    }

    private void Follow(Transform target, float distance)
    {
        bool predicate()
        {
            return Vector3.Distance(transform.position,
            target.position) > distance;
        }

        IEnumerator follow()
        {
            SetState(_walkingState);

            while (predicate())
            {
                _navMeshAgent.SetDestination(target.position);

                yield return new WaitForEndOfFrame();
            }

            SetState(_idlingState);
        }

        StartCoroutine(follow()); 
    }
}
