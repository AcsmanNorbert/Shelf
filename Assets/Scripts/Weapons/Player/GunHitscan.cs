using System.Collections;
using UnityEngine;

public class GunHitscan : Gun
{
    [Space(3)]
    [Header("Hitscan")]
    [SerializeField] protected GameObject trailPrefab;
    [SerializeField] float trailSpeed = 1;

    [SerializeField] private float nearFallofRange = 20;
    [SerializeField] private float farFallofRange = 40;

    protected float NearFallofRange { get => nearFallofRange * CurrentStats.rangeModifier; }
    protected float FarFallofRange { get => farFallofRange * CurrentStats.rangeModifier; }
    
    protected override void FireShot(AmmoType ammo, Vector3 direction)
    {
        DoRaycastHit(ammo, direction, out RaycastHit hit);
        StartCoroutine(SpawnTrail(hit, direction));
    }

    protected void DoRaycastHit(AmmoType ammo, Vector3 direction, out RaycastHit hit)
    {
        float range = CurrentStats.maximumEffectiveRange;
        float distance = range == 0 ? float.MaxValue : range;
        if (Physics.Raycast(headTransform.position, direction, out hit, distance, targetMask) == false) return;
        if (!HitCollision.CheckHitCollision(hit.collider, headTransform, out bool weakPointHit, out Health health)) return;

        float fallofDamage = GetFallofDamage(hit.distance, weakPointHit);
        float damage = GetDamage(fallofDamage, ammo);

        DamageData data = GetDamageData(damage, ammo);

        health.Damage(data, weakPointHit);

        //Debug.Log($"{hit.distance} dist, {fallofDamage} base, {damage} damage");
    }

    private float GetFallofDamage(float distance, bool weakPointHit)
    {
        if (distance < NearFallofRange) return CurrentStats.baseDamage;
        float minimumDamage = CurrentStats.baseDamage * CurrentStats.rangeMultiplier;
        if (distance > FarFallofRange) return minimumDamage;
        float normalizedRange = (distance - NearFallofRange) / (FarFallofRange - NearFallofRange);

        float fallofDamage = normalizedRange * minimumDamage + (1 - normalizedRange) * CurrentStats.baseDamage;
        return fallofDamage;
    }

    protected IEnumerator SpawnTrail(RaycastHit hit, Vector3 direction)
    {
        Vector3 point = hit.point;
        //check if you hit something, if no set default values
        if (hit.collider == null) point = headTransform.position + direction * CurrentStats.maximumEffectiveRange;
        GameObject trail = Instantiate(trailPrefab, shootingPoint.position, Quaternion.identity);
        Vector3 trailPosition = trail.transform.position;

        while (trailPosition != point)
        {
            trailPosition = Vector3.MoveTowards(trailPosition, point, trailSpeed);
            trail.transform.position = trailPosition;

            yield return null;
        }

        TrailRenderer[] trailRenderers = trail.GetComponentsInChildren<TrailRenderer>();
        float time = trail.GetComponent<TrailRenderer>().time;
        foreach (TrailRenderer renderer in trailRenderers)
            time = Mathf.Clamp(renderer.time, time, renderer.time);
        Destroy(trail, time);
    }

    private void OnDrawGizmos()
    {
        if (headTransform == null || !activeGun) return;
        Gizmos.color = Color.red;
        Vector3 start = headTransform.position;
        Vector3 end = headTransform.position + headTransform.forward * CurrentStats.maximumEffectiveRange;
        Gizmos.DrawLine(start, end);
    }
}
