using System;
using UnityEngine;

[Serializable]
public class NPCReplica
{
    [SerializeField] private string _name = "?";
    [SerializeField] private AudioClip[] _audioClips;

    public string Name => _name;

    public AudioClip GetRandomAudioClip()
    {
        return _audioClips
            [UnityEngine.Random.Range(0, _audioClips.Length)];
    }
}
