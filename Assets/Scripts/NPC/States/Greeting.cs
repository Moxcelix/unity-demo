using System;
using System.Collections;
using UnityEngine;

using Random = UnityEngine.Random;

[Serializable]
public class Greeting : NPCState
{
    private const string c_animationName = "greeting";
    private const float c_minWavingTime = 2.0f;
    private const float c_maxWavingTime = 4.0f;
    public override void Start()
    {
        LookAtPlayer = true;

        _npc.PlayAnimation(c_animationName);
        _npc.StartCoroutine(Update());
    }

    private IEnumerator Update()
    {
        var time = Random.Range(c_minWavingTime, c_maxWavingTime);

        IsCompleted = false;

        yield return new WaitForSeconds(time);

        IsCompleted = true;
    }
}
