using System.Collections;
using UnityEngine;

[System.Serializable]
public class Walking : NPCState
{
    private const string c_animationName = "walking";

    public Transform Target { get; set; }
    public float Tolerance { get; set; }

    public override void Start()
    {
        IsCompleted = false;
        LookAtPlayer = false;

        _npc.PlayAnimation(c_animationName);
        _npc.StartCoroutine(Update());
    }

    private IEnumerator Update()
    {
        bool predicate()
        {
            return Vector3.Distance(_npc.transform.position,
            Target.position) > Tolerance;
        }

        while (predicate())
        {
            _npc.GoTo(Target.position);

            yield return new WaitForEndOfFrame();
        }

        IsCompleted = true;
    }
}
