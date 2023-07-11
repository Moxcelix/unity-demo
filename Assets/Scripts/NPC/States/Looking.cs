using System.Collections;
using UnityEngine;

[System.Serializable]
public class Looking : NPCState
{
    private const string c_animationName = "looking";
    private const float c_timeOutTime = 6.0f;

    public Transform LookTarget { get; set; } = null!;

    public override void Start()
    {
        IsCompleted = false;
        LookAtPlayer = false;

        _npc.FollowTarget(LookTarget);
        _npc.PlayAnimation(c_animationName);
        _npc.StartCoroutine(TimeOut());
    }

    private IEnumerator TimeOut()
    {
        yield return new WaitForSeconds(c_timeOutTime);

        IsCompleted = true;
    }
}