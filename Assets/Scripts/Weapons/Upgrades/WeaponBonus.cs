using System;

[Serializable]
public class WeaponBonus
{
    public EnhancementType type;
    public string upgradeName;
    public string description;
    public float amount;
}

[Serializable]
public enum EnhancementType
{
    Damage,
    ReloadTime,
    FireRate,
    ReadyTime,
    Recoil,
    Range,
    Spread,
    Special
}
