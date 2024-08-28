using System;
using UnityEngine;

[Serializable]
public class WeaponStats
{
    public string weaponName = "New Weapon";

    [Space(3), Header("Base stats")]
    public float baseDamage = 1;
    public float headshotMult = 1.5f;
    [Min(1)] public int magazineSize = 5;
    public float reloadSpeed = 1;
    public float fireRate = 0.5f;
    public float readyTime = 1;
    public float zoomFOW = 40;

    [Space(3), Header("Recoil stats")]
    [NonSerialized] public float recoilModifier = 1;
    public float recoilX = 1.2f;
    public float recoilY = 1.5f;
    public float snappiness = 4;
    public float returnSpeed = 2;

    [Space(3), Header("Range stats")]
    [NonSerialized] public float rangeModifier = 1;
    public float rangeMultiplier = 0.8f;

    [Space(3), Header("Bullet spread")]
    [NonSerialized] public float spreadModifier = 1;
    public float verticalSpread = 0f;
    public float horizontalSpread = 0f;
    public float maximumEffectiveRange = 80;

    public WeaponStats Copy()
    {
        WeaponStats newStats = new();

        newStats.weaponName = weaponName;
        newStats.baseDamage = baseDamage;
        newStats.headshotMult = headshotMult;
        newStats.magazineSize = magazineSize;
        newStats.reloadSpeed = reloadSpeed;
        newStats.fireRate = fireRate;
        newStats.readyTime = readyTime;
        newStats.zoomFOW = zoomFOW;

        newStats.recoilModifier = recoilModifier;
        newStats.recoilX = recoilX;
        newStats.recoilY = recoilY;
        newStats.snappiness = snappiness;
        newStats.readyTime = readyTime;

        newStats.rangeModifier = rangeModifier;
        newStats.rangeMultiplier = rangeMultiplier;

        newStats.spreadModifier = spreadModifier;
        newStats.verticalSpread = verticalSpread;
        newStats.horizontalSpread = horizontalSpread;
        newStats.maximumEffectiveRange = maximumEffectiveRange > 0 ? maximumEffectiveRange : 9999;

        return newStats;
    }
}
