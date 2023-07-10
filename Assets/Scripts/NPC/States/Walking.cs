[System.Serializable]
public class Walking : NPCState
{
    private const string c_animationName = "walking";

    public override void Start()
    {
        IsCompleted = false;
        LookAtPlayer = false;

        _npc.PlayAnimation(c_animationName);
    }
}
