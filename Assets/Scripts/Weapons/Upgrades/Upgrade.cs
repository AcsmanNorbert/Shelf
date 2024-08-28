using UnityEngine;

public abstract class Upgrade : ScriptableObject
{
    [SerializeField] WeaponBonus[] bonus;
    [SerializeField] string upgradeName;
    [SerializeField, TextArea(3, 3)] string description;
    [SerializeField, Min(1)] int maxStacks = 1;
    [SerializeField] float duration;
    [SerializeField] bool doStacksDecay = false;
    [SerializeField] float decayModifier = 1;

    public WeaponBonus[] Bonus { get => bonus; }
    public int MaxStacks { get => maxStacks; }
    public float Duration { get => duration; }
    public bool DoStacksDecay { get => doStacksDecay; }
    public float DecayModifier { get => decayModifier; }

    protected virtual void Apply(WeaponStatHandler handler) => handler.ApplyUgprade(this); 

    public virtual void OnKill(WeaponStatHandler handler, DamageData data, Health targetHealth) { }
    public virtual void OnDamage(WeaponStatHandler handler, DamageData data, Health targetHealth) { }
    public virtual void OnFire(WeaponStatHandler handler, AmmoType ammo) { }
}
