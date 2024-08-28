using System;
using UnityEngine;

[RequireComponent(typeof(FixedPoints))]
public abstract class Health : MonoBehaviour, IDamageable
{
    [Header("Debug")]
    [SerializeField] bool invulerable;
    [SerializeField] float startingHealth = 10;
    public float StartingHealth { get => startingHealth; }
    float health; 

    protected bool isDead = false;
    public bool IsDead { get => isDead; }

    [SerializeField] LayerMask reverseTargetMask;
    public LayerMask ReverseTargetMask { get => reverseTargetMask; }
    public FixedPoints FixedPoints { get => GetComponent<FixedPoints>(); }

    public Action<HitData> OnInflictingDamage;
    public Action<HitData> OnKill;
    public Action<HitData> OnGettingDamaged;
    public Action<HitData> OnDeath;

    private void Start()
    {
        health = StartingHealth;
    }

    public virtual void Damage(DamageData damageData, bool weakPointHit)
    {
        if (isDead) return;

        float damage = weakPointHit ? damageData.damage * damageData.headshotMult : damageData.damage;
        if (!invulerable) health -= damage;

        HitData hitData = GetHitData(damageData.type, weakPointHit);

        damageData.weapon.OnDamageTrigger?.Invoke(damageData, this);
        damageData.inflicter.OnInflictingDamage?.Invoke(hitData);
        damageData.inflicter.OnGettingDamaged?.Invoke(hitData);
        if (health <= 0)
        {
            damageData.weapon.OnKillTrigger?.Invoke(damageData, this);
            damageData.inflicter.OnKill?.Invoke(hitData);
            damageData.inflicter.OnDeath?.Invoke(hitData);
            Death();
        }
    }

    public virtual void Death()
    {
        PlayEffect(EntityEvent.Dead);
        isDead = true;
    }

    public void PlayEffect(EntityEvent entityEvent)
    {
        IEntityEffect[] players = GetComponents<IEntityEffect>();
        foreach (IEntityEffect player in players)
            player.DoEffect(entityEvent);
    }

    public HitData GetHitData(AmmoType type, bool weakPointHit)
    {
        HitData data = new();
        data.inflicted = this;
        data.type = type;
        data.weakPointHit = weakPointHit;
        return data;
    }
}
