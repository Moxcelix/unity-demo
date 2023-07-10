[System.Serializable]
public abstract class NPCState
{
    public bool IsCompleted { get; protected set; }
    public bool LookAtPlayer { get; protected set; }

    protected NPC _npc;

    public void Initialize(NPC npc)
    {
        _npc = npc;
    }

    public abstract void Start();

}
