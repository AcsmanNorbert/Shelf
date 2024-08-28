using UnityEngine;

[CreateAssetMenu(fileName = "New Upgrade", menuName = "Weapon Upgrades/Rampage")]
public class U_Rampage : Upgrade
{
    public override void OnKill(WeaponStatHandler handler, DamageData data, Health targetHealth)
    {
        base.OnKill(handler, data, targetHealth);
        Apply(handler);
    }
}