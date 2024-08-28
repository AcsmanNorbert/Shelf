using System;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField] protected Transform shootingPoint;
    public Vector3 ShootingPointPosition { get => shootingPoint.position; }
    protected Health health;

    protected virtual void Awake()
    {
        if (health == null) health = GetComponentInParent<Health>();
        targetMask = ~health.ReverseTargetMask;
    }

    protected LayerMask targetMask;
    public LayerMask TargetMask { get => targetMask; }
    protected LayerMask playerMask;

    #region EVENTS
    public Action<DamageData, Health> OnDamageTrigger;
    public Action<DamageData, Health> OnKillTrigger;
    #endregion

    #region PLAYEFFECT
    protected void PlayEffects(WeaponEvent weaponEvent)
    {
        PlayEffectsSelf(weaponEvent);
        PlayEffectsChild(weaponEvent);
        PlayEffectsParent(weaponEvent);
    }
    protected void PlayEffectsSelf(WeaponEvent weaponEvent)
    {
        foreach (IWeaponEffect player in GetComponents<IWeaponEffect>())
            player.DoEffect(weaponEvent);
    }

    protected void PlayEffectsChild(WeaponEvent weaponEvent)
    {
        foreach (IWeaponEffect player in GetComponentsInChildren<IWeaponEffect>())
            player.DoEffect(weaponEvent);
    }

    protected void PlayEffectsParent(WeaponEvent weaponEvent)
    {
        foreach (IWeaponEffect player in GetComponentsInParent<IWeaponEffect>())
            player.DoEffect(weaponEvent);
    }
    #endregion
}
