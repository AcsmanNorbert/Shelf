using UnityEngine;
using UnityEngine.Events;

public class GunHitscan : Gun
{
    public UnityEvent GunFired;
    protected override void OnGunFired(AmmoType ammo)
    {
        RaycastHit hit;
        Transform headTransform = PlayerMovement.GetHeadTransform();
        if (Physics.Raycast(headTransform.position, headTransform.forward, out hit, float.MaxValue, ~loadoutManager.reverseTargetMask))
        {
            if (hit.collider.TryGetComponent(out Health health))
                health.Damage(damage * GetDamageMultiplyer(ammo));
        }
        GunFired?.Invoke();
    }
}