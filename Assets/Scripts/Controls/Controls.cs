using Core.Player;
using UnityEngine;

public class Controls : MonoBehaviour, IControls
{
    [Header("Character controls")]
    [SerializeField] private KeyCode _forwardKey = KeyCode.W;
    [SerializeField] private KeyCode _backKey = KeyCode.S;
    [SerializeField] private KeyCode _rightKey = KeyCode.D;
    [SerializeField] private KeyCode _leftKey = KeyCode.A;
    [SerializeField] private KeyCode _jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode _runKey = KeyCode.LeftControl;
    [SerializeField] private KeyCode _leaveKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode _pauseKey = KeyCode.Escape;
    [SerializeField] private float _mouseSensitivity = 2;

    public float RotationDeltaX { get; private set; }
    public float RotationDeltaY { get; private set; }
    public bool MoveForward { get; private set; }
    public bool MoveBack { get; private set; }
    public bool MoveRight { get; private set; }
    public bool MoveLeft { get; private set; }
    public bool IsRunning { get; private set; }
    public bool IsJumping { get; private set; }
    public bool Leave { get; private set; }

    private bool _isPause = false;

    public void Initialize()
    {
        _isPause = false;
        MouseController.SetVisibility(false);
    }

    private void Update()
    {
        RotationDeltaX = Input.GetAxis("Mouse X") * _mouseSensitivity;
        RotationDeltaY = Input.GetAxis("Mouse Y") * _mouseSensitivity;

        MoveForward = Input.GetKey(_forwardKey);
        MoveBack = Input.GetKey(_backKey);
        MoveRight = Input.GetKey(_rightKey);
        MoveLeft = Input.GetKey(_leftKey);
        IsRunning = Input.GetKey(_runKey);
        IsJumping = Input.GetKey(_jumpKey);
        Leave = Input.GetKeyDown(_leaveKey);

        if (Input.GetKeyDown(_pauseKey))
        {
            _isPause = !_isPause;

            MouseController.SetVisibility(_isPause);
        }
    }
}
