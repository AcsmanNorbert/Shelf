using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GunHitEffect : MonoBehaviour
{
    [SerializeField] Health health;
    [SerializeField] AudioClip normalHit;
    [SerializeField] AudioClip weakPointHit;

    [SerializeField] AudioClip normalKill;
    [SerializeField] AudioClip weakPointKill;

    AudioSource audioSource;
    private void OnValidate()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        health.OnInflictingDamage += OnInflictingDamage;
        health.OnKill += OnKill;
    }

    void OnInflictingDamage(HitData hitData) => HitSound(hitData);

    bool canPlay = true;
    public void HitSound(HitData hitData)
    {   
        if (!canPlay) return;

        //Debug.Log(hitData.weakPointHit);
        if (hitData.weakPointHit) audioSource.PlayOneShot(weakPointHit);
        else audioSource.PlayOneShot(normalHit);
        StartCoroutine(Delay());
    }

    IEnumerator Delay()
    {
        canPlay = false;
        yield return new WaitForSeconds(0.02f);
        canPlay = true;
    }

    void OnKill(HitData hitData) => KillSound(hitData);

    void KillSound(HitData hitData)
    {
        if (hitData.weakPointHit) audioSource.PlayOneShot(weakPointKill);
        else audioSource.PlayOneShot(normalKill);
    }
}
