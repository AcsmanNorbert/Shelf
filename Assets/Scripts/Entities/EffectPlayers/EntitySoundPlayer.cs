using System.Collections.Generic;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
public class EntitySoundPlayer : MonoBehaviour, IEntityEffect
{
    AudioSource player;
    public AudioSource Player => player;
    [SerializeField] List<EntityEventAudio> eventAudio;

    void Start()
    {
        player = GetComponent<AudioSource>();
    }

    public void DoEffect(EntityEvent eventType)
    {
        List<AudioClip> clipList = new();
        foreach (EntityEventAudio audio in eventAudio)
        {
            if (audio.eventType != eventType) continue;
            if (audio.randomPool) clipList.Add(audio.clip);
            else player.PlayOneShot(audio.clip);
        }
        if (clipList.Count == 0) return;
        int randomClip = Random.Range(0, clipList.Count);
        player.PlayOneShot(clipList[randomClip]);
    }
}

[Serializable]
class EntityEventAudio
{
    public EntityEvent eventType;
    public AudioClip clip;
    public bool randomPool;
}
