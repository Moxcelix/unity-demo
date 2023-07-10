using Core.Interactive;
using Core.Player;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class NPC : MonoBehaviour, INPCInteractable, ISitable
{
    private const float c_translateTolerance = 0.05f;

    [SerializeField] private string _hint;
    [SerializeField] private float _rotatinonSpeed;
    [SerializeField] private float _translateSpeed;
    [SerializeField] private Transform _playerTransform;

    [Header("NPC States")]
    [SerializeField] private Idling _idling;
    [SerializeField] private Greeting _greeting;
    [SerializeField] private Walking _walking;
    [SerializeField] private PlayingPiano _playingPiano;
    [SerializeField] private Looking _looking;

    private NavMeshAgent _navMeshAgent;
    private Transform _lookAtTarget;
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
        _animator = GetComponent<Animator>();

        _state = _idling;

        _idling.Initialize(this);
        _greeting.Initialize(this);
        _walking.Initialize(this);
        _playingPiano.Initialize(this);
        _looking.Initialize(this);

        _idling.Start();
    }

    private void Update()
    {
        IsAvailable = _state.IsCompleted && !IsSitting;

        if (_state != _idling && _state.IsCompleted)
        {
            SetState(_idling);
        }

        if (_state == _playingPiano)
        {
            if (_playingPiano.Piano.IsAvailable)
            {
                _playingPiano.Piano.SeatPlace.Free();
            }
            else
            {
                _playingPiano.Piano.SeatPlace.Take(this);
            }
        }

        if (_lookAtTarget != null)
        {
            LookAt(_lookAtTarget.position, Time.deltaTime);
        }
    }

    public void SetLookTarget(Transform target)
    {
        _looking.LookTarget = target;

        SetState(_looking);
    }

    public void PlayPiano(Piano piano)
    {
        _playingPiano.Piano = piano;

        SetState(_playingPiano);

        piano.Play();
    }

    public void StayStill()
    {
        _navMeshAgent.ResetPath();

        SetState(_idling);
    }

    public void InteractWith(INPCInteractable interactable)
    {
        if (!IsAvailable)
        {
            return;
        }

        StartCoroutine(GoToAndDo(
            interactable.Target.position,
            interactable.Range, () =>
            {
                StayStill();
                interactable.Interact(this);
            }));
    }

    public void Interact(NPC interactor)
    {
        _navMeshAgent.ResetPath();

        SetState(_greeting);
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

        SetState(_idling);
    }

    private void SetState(NPCState state)
    {
        _state = state;

        _state.Start();

        HandleStateTransition(_state);
    }

    private void HandleStateTransition(NPCState state)
    {
        if (state.LookAtPlayer)
        {
            _lookAtTarget = _playerTransform;
        }
        else if (state == _looking)
        {
            _lookAtTarget = _looking.LookTarget;
        }
        else if (state == _playingPiano)
        {
            _lookAtTarget = _playingPiano.Piano.transform;
        }
        else
        {
            _lookAtTarget = null;
        }
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
            SetState(_walking);
        }

        while (distance > tolerance)
        {
            distance = Vector3.Distance(position, transform.position);

            yield return new WaitForEndOfFrame();
        }

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
}
