using Core.Interactive;
using Core.ViewProber;
using TMPro;
using UnityEngine;

public class PlayerIO : MonoBehaviour, IInteractor
{
    private Raycaster _raycaster;
    private NPC _npc;

    [SerializeField] private Transform _headTransform;
    [SerializeField] private GameObject _cursor;
    [SerializeField] private TextMeshProUGUI _hintTextRenderer;

    public GameObject GameObject => gameObject;
    public NPC CurrentNPC => _npc;

    public void Initialize(Raycaster raycaster, NPC npc)
    {
        _raycaster = raycaster;
        _npc = npc;
    }

    private void Update()
    {
        var hit = _raycaster.CheckHit<INPCInteractable>();

        if (hit == null || !hit.IsAvailable)
        {
            _cursor.SetActive(false);

            return;
        }

        _cursor.SetActive(true);

        _hintTextRenderer.text = hit.HintText;

        if (Input.GetKeyDown(KeyCode.E))
        {
            _npc.InteractWith(hit);
        }
    }
}
