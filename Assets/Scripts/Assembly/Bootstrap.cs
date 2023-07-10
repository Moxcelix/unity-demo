using Core.Player;
using Core.ViewProber;
using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private PlayerBody _player;
    [SerializeField] private Controls _controls;
    [SerializeField] private PlayerIO _playerIO;
    [SerializeField] private NPC _npc;

    private PlayerController _playerController;

    private void Awake()
    {
        var raycaster = new Raycaster(_player.HeadTransform, 3.0f);
        _playerController = new PlayerController(_player);
        _playerController.IsAvailable = true;

        _controls.Initialize();
        _playerIO.Initialize(raycaster, _npc);
    }

    private void Update()
    {
        _playerController?.Update(_controls);
    }
}
