using System;

[Serializable]
public class Idling : NPCState
{
    private const string c_animationName = "idling";

    public override void Start()
    {
        IsCompleted = true;
        LookAtPlayer = false;

        _npc.StayStill();
        _npc.PlayAnimation(c_animationName);
    }
}
