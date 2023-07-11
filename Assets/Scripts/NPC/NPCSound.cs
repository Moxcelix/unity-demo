using System.Linq;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class NPCSound : MonoBehaviour
{
    [SerializeField] private NPCReplica[] _replicas;

    private AudioSource _audioSource;

    public void Initialize()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void Say(string key)
    {
        var replica = from r in _replicas
                      where r.Name == key
                      select r;

        if (replica is null)
        {
            return;
        }

        _audioSource.PlayOneShot(
            replica.First().GetRandomAudioClip());
    }
}
