using UnityEngine;

public class GunProjectile : Gun
{
    [Space(3)]
    [Header("Projectile")]
    [SerializeField] protected GameObject projectilePrefab;
    [SerializeField] float projectileSpeed = 15f;
    [SerializeField] float maxDisctance = 50f;

    protected override void FireShot(AmmoType ammo, Vector3 direction)
    {
        ShootProjectile(ammo, direction);
    }

    protected void ShootProjectile(AmmoType ammo, Vector3 direction)
    {
        GameObject prefab = Instantiate(projectilePrefab);
        prefab.transform.position = shootingPoint.position;
        prefab.transform.rotation = Quaternion.LookRotation(direction);
        Projectile projectile = prefab.GetComponent<Projectile>();

        float damage = GetDamage(CurrentStats.baseDamage, ammo);

        DamageData data = GetDamageData(damage, ammo);

        projectile.SetupProjectile(data, projectileSpeed, maxDisctance);
    }

    private void OnDrawGizmos()
    {
        if (headTransform == null || !activeGun) return;
        Gizmos.color = Color.red;
        Vector3 start = headTransform.position;
        Vector3 end = headTransform.position + headTransform.forward * maxDisctance;
        Gizmos.DrawLine(start, end);
    }
}