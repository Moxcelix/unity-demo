using Core.Interactive;
using Core.Player;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class NPC : MonoBehaviour, IInteractable, ISitable
{
    private const float c_translateTolerance = 0.05f;

    [SerializeField] private string _hint;
    [SerializeField] private float _rotatinonSpeed;
    [SerializeField] private float _translateSpeed;
    [SerializeField] private Transform _lookAtTarget;

    [Header("NPC States")]
    [SerializeField] private Idling _idling;
    [SerializeField] private Greeting _greeting;
    [SerializeField] private Walking _walking;
    [SerializeField] private PlayingPiano _playingPiano;

    private NavMeshAgent _navMeshAgent;
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

        _idling.Start();
    }

    private void Update()
    {
        IsAvailable = _state.IsCompleted;

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

                LookAt(_playingPiano.Piano.transform.position);
            }
        }

        if (_state.LookAtPlayer)
        {
            LookAt(_lookAtTarget.position);
        }
    }

    public void PlayPiano(Piano piano)
    {
        SetState(_playingPiano);

        _playingPiano.Piano = piano;

        piano.Play();
    }

    public void InteractWith(IInteractable interactable)
    {
        if(!IsAvailable)
        {
            return;
        }

        StartCoroutine(GoToAndDo(
            interactable.Target.position,
            interactable.Range, () =>
            interactable.Interact(this)));
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
            c_translateTolerance, () => {
                IsSitting = false;
                _navMeshAgent.enabled = true;
            }));

        SetState(_idling);
    }

    private void SetState(NPCState state)
    {
        _state = state;

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
        Debug.Log("Сел");

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

    private void LookAt(Vector3 position)
    {
        var target = new Vector3(position.x,
                transform.position.y, position.z);

        var targetRotation = Quaternion.LookRotation(
            target - transform.position);

        transform.rotation = Quaternion.Slerp(
            transform.rotation, targetRotation,
            _rotatinonSpeed * Time.deltaTime);
    }
}
