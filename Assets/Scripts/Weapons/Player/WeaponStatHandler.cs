using System;
using System.Collections.Generic;
using UnityEngine;

public class WeaponStatHandler : MonoBehaviour
{
    [SerializeField] WeaponStats baseStats;
    public WeaponStats BaseStats { get => baseStats; }

    WeaponStats currentStats;
    public WeaponStats CurrentStats { get => currentStats; }

    [SerializeField] List<Upgrade> weaponUpgrades;
    Dictionary<Upgrade, UpgradeState> activeUpgrades = new();
    public Dictionary<Upgrade, UpgradeState> ActiveUpgrades { get => activeUpgrades; }
    [SerializeField] bool debugLogStacks;

    private void Awake()
    {
        Gun gun = GetComponent<Gun>();
        gun.OnKillTrigger += OnWeaponKillTrigger;
        gun.OnDamageTrigger += OnWeaponDamageTrigger;
        gun.OnFireTrigger += OnWeaponFireTrigger;
        currentStats = baseStats.Copy();
    }

    private void OnValidate()
    {
        currentStats = baseStats.Copy();
    }

    private void OnWeaponKillTrigger(DamageData data, Health health)
    {
        foreach (Upgrade upgrade in weaponUpgrades)
            upgrade.OnKill(this, data, health);
    }

    private void OnWeaponDamageTrigger(DamageData data, Health health)
    {
        foreach (Upgrade upgrade in weaponUpgrades)
            upgrade.OnDamage(this, data, health);
    }

    private void OnWeaponFireTrigger(AmmoType ammo)
    {
        foreach (Upgrade upgrade in weaponUpgrades)
            upgrade.OnFire(this, ammo);
    }

    public void ApplyUgprade(Upgrade upgrade)
    {
        SetUpgradeActiveDuration(upgrade);
    }

    private void SetUpgradeActiveDuration(Upgrade upgrade)
    {
        float duration = upgrade.Duration;
        int activeCount = activeUpgrades.Count;
        if (activeUpgrades != null)
        {
            if (activeUpgrades.TryGetValue(upgrade, out UpgradeState state))
            {
                if (duration > state.timer) state.timer = duration;
                if (state.stack < upgrade.MaxStacks)
                {
                    state.stack++;
                    ApplyUpgrade(upgrade, 1);
                    if (debugLogStacks) Debug.Log($"{upgrade.name} + {state.stack}");
                    OnBonusStackGained?.Invoke(upgrade, state);
                }
                return;
            }
        }
        UpgradeState newState = new();
        newState.stack = 1;
        newState.timer = duration;
        activeUpgrades.Add(upgrade, newState);
        ApplyUpgrade(upgrade, 1);
        OnBonusGained?.Invoke(upgrade, newState);
    }

    private void ApplyUpgrade(Upgrade upgrade, int stacks)
    {
        WeaponBonus[] bonuses = upgrade.Bonus;
        WeaponStats newStats = currentStats.Copy();
        if (bonuses == null)
        {
            Debug.Log($"No enhancements have been set up in the inspector in: {name}");
            return;
        }
        else
        {
            foreach (WeaponBonus bonus in bonuses)
            {
                var bonusValue = bonus.amount * stacks;
                switch (bonus.type)
                {
                    case EnhancementType.Damage:
                        newStats.baseDamage += BaseStats.baseDamage * bonusValue;
                        break;
                    case EnhancementType.ReloadTime:
                        newStats.reloadSpeed += BaseStats.reloadSpeed * bonusValue;
                        break;
                    case EnhancementType.FireRate:
                        newStats.fireRate += BaseStats.fireRate * bonusValue;
                        break;
                    case EnhancementType.ReadyTime:
                        newStats.readyTime += BaseStats.readyTime * bonusValue;
                        break;
                    case EnhancementType.Recoil:
                        newStats.recoilModifier += BaseStats.recoilModifier * bonusValue;
                        break;
                    case EnhancementType.Range:
                        newStats.rangeModifier += BaseStats.rangeModifier * bonusValue;
                        break;
                    case EnhancementType.Spread:
                        newStats.spreadModifier += baseStats.spreadModifier * bonusValue;
                        break;
                    default:
                        break;
                }
            }
        }
        currentStats = newStats;
    }

    private void UpgradeTimerUpdate()
    {
        if (activeUpgrades.Count == 0) return;

        List<Upgrade> removeables = new();
        foreach ((Upgrade upgrade, UpgradeState state) in activeUpgrades)
        {
            state.timer -= Time.deltaTime;
            if (state.timer <= 0)
            {
                if (upgrade.DoStacksDecay)
                {
                    ApplyUpgrade(upgrade, -1);
                    state.stack--;
                    if (state.stack == 0)
                    {
                        removeables.Add(upgrade);
                        OnBonusLost?.Invoke(upgrade, state);
                    }
                    else
                    {
                        state.timer = upgrade.Duration * upgrade.DecayModifier;
                        OnBonusStackLost?.Invoke(upgrade, state);
                    }
                }
                else
                {
                    ApplyUpgrade(upgrade, -state.stack);
                    removeables.Add(upgrade);
                }
                if (debugLogStacks) Debug.Log($"{upgrade.name} + {state.stack}");
            }
        }

        foreach (Upgrade u in removeables)
        {
            activeUpgrades.Remove(u);
        }
    }

    private void Update()
    {
        UpgradeTimerUpdate();
    }

    public Action<Upgrade, UpgradeState> OnBonusGained;
    public Action<Upgrade, UpgradeState> OnBonusLost;
    public Action<Upgrade, UpgradeState> OnBonusStackGained;
    public Action<Upgrade, UpgradeState> OnBonusStackLost;
}

public class UpgradeState
{
    public float timer;
    public int stack;
}