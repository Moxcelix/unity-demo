using Core.Player;
using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private PlayerBody _player;
    [SerializeField] private Controls _controls;
    [SerializeField] private PlayerIO _playerIO;

    private PlayerController _playerController;

    private void Awake()
    {
        _playerController = new PlayerController(_player);
        _playerController.IsAvailable = true;

        _controls.Initialize();
        _playerIO.Initialize();
    }

    private void Update()
    {
        _playerController?.Update(_controls);
    }
}
