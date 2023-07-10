using Core.Interactive;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class NPC : MonoBehaviour, IInteractable
{
    [SerializeField] private string _hint;
    [SerializeField] private float _rotatinonSpeed;
    [SerializeField] private Transform _lookAtTarget;

    [Header("NPC States")]
    [SerializeField] private Idling _idling;
    [SerializeField] private Greeting _greeting;

    private NavMeshAgent _navMeshAgent;
    private Animator _animator;
    private NPCState _state;

    public GameObject GameObject => gameObject;
    public string HintText => _hint;
    public bool IsAvailable { get; private set; }

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();

        _state = _idling;

        _idling.Initialize(this);
        _greeting.Initialize(this);

        _idling.Start();
    }

    private void Update()
    {
        IsAvailable = _state.IsCompleted;

        if(_state != _idling && _state.IsCompleted)
        {
            SetState(_idling);
        }

        if (_state.LookAtPlayer)
        {
            var target = new Vector3(
                _lookAtTarget.position.x,
                transform.position.y,
                _lookAtTarget.position.z);
            var targetRotation = Quaternion.LookRotation(
                target - transform.position);
            transform.rotation = Quaternion.Slerp(
                transform.rotation, targetRotation, 
                _rotatinonSpeed * Time.deltaTime);
        }
    }

    public void InteractWith(IInteractable interactable)
    {
        _navMeshAgent.SetDestination(
            interactable.GameObject.transform.position);
    }

    public void Interact(IInteractor interactor)
    {
        _navMeshAgent.ResetPath();

        SetState(_greeting);
    }

    public void PlayAnimation(string arg)
    {
        _animator.SetTrigger(arg);
    }

    private void SetState(NPCState state)
    {
        _state = state;

        _state.Start();
    }
}
