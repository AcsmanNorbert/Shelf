using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
public class WeaponEventSoundPlayer : MonoBehaviour, IWeaponEffect
{
    AudioSource player;
    public AudioSource Player => player;
    [SerializeField] List<WeaponEventAudio> weaponAudio;

    void Start()
    {
        player = GetComponent<AudioSource>();
    }

    public void DoEffect(WeaponEvent weaponEffect)
    {
        List<AudioClip> clipList = new();
        foreach (WeaponEventAudio audio in weaponAudio)
        {
            if (audio.eventType != weaponEffect) continue;
            if (audio.randomPool) clipList.Add(audio.clip); 
            else player.PlayOneShot(audio.clip);
        }
        if (clipList.Count == 0) return;
        int randomClip = Random.Range(0, clipList.Count);
        player.PlayOneShot(clipList[randomClip]);
    }
}

[Serializable]
class WeaponEventAudio
{
    public WeaponEvent eventType;
    public AudioClip clip;
    public bool randomPool;
}
