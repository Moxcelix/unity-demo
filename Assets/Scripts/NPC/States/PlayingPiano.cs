[System.Serializable]
public class PlayingPiano : NPCState
{
    private const string c_animationName = "playing_piano";

    public Piano Piano { get; set; } = null!;

    public override void Start()
    {
        IsCompleted = false;
        LookAtPlayer = false;

        _npc.FollowTarget(Piano.transform);
        _npc.PlayAnimation(c_animationName);
    }
}

