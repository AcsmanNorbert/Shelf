using System.Collections;
using UnityEngine;

[RequireComponent(typeof(NavMeshAimConstrain))] 
public class ProjectileAttack : Weapon
{
    //[SerializeField] WeaponStats currentStats;
    //public WeaponStats CurrentStats { get => currentStats.Copy(); }

    [SerializeField] Transform headTransform;
    [SerializeField] EnemyNavMesh navMesh;

    [Space(3), Header("Base stats")]
    [SerializeField] float baseDamage;
    [SerializeField] float fireRate;
    [SerializeField, Min(1)] int fireAmount;

    [Space(3), Header("Projectile")]
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] float projectileSpeed = 15f;
    [SerializeField] float maxDisctance = 50f;
    [SerializeField] float horizontalAccuracy;
    [SerializeField] float verticalAccuracy;

    [Space(3)]
    [SerializeField] bool showGizmos;

    int magazine;

    public void OnAttack()
    {
        magazine = fireAmount;
        PlayEffectsParent(WeaponEvent.Fire);
        StartCoroutine(Shoot());
    }

    IEnumerator Shoot()
    {
        while (magazine > 0)
        {
            if (health.IsDead) yield break;

            Vector3 direction = SpreadDirection(shootingPoint.forward);
            ShootProjectile(direction);

            PlayEffectsChild(WeaponEvent.Fire);
            magazine--;
            if (magazine > 0) yield return new WaitForSeconds(fireRate);
        }
    }

    protected void ShootProjectile(Vector3 direction)
    {
        GameObject prefab = Instantiate(projectilePrefab);
        prefab.transform.position = shootingPoint.position;
        prefab.transform.rotation = Quaternion.LookRotation(direction);
        Projectile projectile = prefab.GetComponent<Projectile>();

        float damage = GetDamage(baseDamage);

        DamageData data = GetDamageData(damage, AmmoType.Basic);

        projectile.SetupProjectile(data, projectileSpeed, maxDisctance);
    }

    Vector3 SpreadDirection(Vector3 direction)
    {
        float horizontalSpread = Random.Range(-horizontalAccuracy, horizontalAccuracy);
        float verticalSpread = Random.Range(-verticalAccuracy, verticalAccuracy);

        direction += shootingPoint.right * horizontalSpread;
        direction += shootingPoint.up * verticalSpread;
        return direction;
    }

    protected virtual float GetDamage(float baseDamage)
    {
        float damage = baseDamage;
        return damage;
    }

    protected DamageData GetDamageData(float damage, AmmoType ammo)
    {
        DamageData data = new();
        data.damage = damage;
        data.headshotMult = 1;
        data.type = ammo;
        data.inflicter = health;
        data.weapon = this;

        return data;
    }

    private void OnDrawGizmos()
    {
        if (headTransform == null || !showGizmos) return;
        Gizmos.color = Color.green;
        Vector3 start = headTransform.position;
        Vector3 end = headTransform.position + headTransform.forward * maxDisctance;
        Gizmos.DrawLine(start, end);
        Gizmos.color = Color.red;
        start = shootingPoint.position;
        end = shootingPoint.position + shootingPoint.forward * maxDisctance;
        Gizmos.DrawLine(start, end);
    }
}
