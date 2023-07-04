using Core.Interactive;
using Core.ViewProber;
using TMPro;
using UnityEngine;

public class PlayerIO : MonoBehaviour
{
    private Raycaster _viewProbe;

    [SerializeField] private Transform _headTransform;
    [SerializeField] private GameObject _cursor;
    [SerializeField] private TextMeshProUGUI _hintTextRenderer;

    public void Initialize()
    {
        _viewProbe = new Raycaster(_headTransform, 3.0f);
    }

    private void Update()
    {
        var hit = _viewProbe.CheckHit<IInteractable>();
            
        _cursor.SetActive(hit is not null);

        if(hit is null)
        {
            return;
        }

        _hintTextRenderer.text = hit.HintText;

        if (Input.GetKeyDown(KeyCode.E))
        {
            hit.Interact();
        }
    }
}
