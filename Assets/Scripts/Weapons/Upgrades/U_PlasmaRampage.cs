using UnityEngine;

[CreateAssetMenu(fileName = "New Upgrade", menuName = "Weapon Upgrades/FireTrigger")]
public class U_FireTrigger : Upgrade
{
    public override void OnFire(WeaponStatHandler handler, AmmoType ammo)
    {
        base.OnFire(handler, ammo);
        Apply(handler);
    }
}