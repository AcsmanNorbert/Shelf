using UnityEngine;

public class GunHitscanPellet : GunHitscan
{
    [Space(3)]
    [Header("Pellet Fire")]
    [SerializeField] int pelletAmount = 5;

    protected override void FireShot(AmmoType ammo, Vector3 direction)
    {
        float pelletSpread = Mathf.Max(CurrentStats.horizontalSpread, CurrentStats.verticalSpread);
        Vector3[] directions = ConeDrawer.GetConePointsAngle(headTransform, pelletSpread, pelletAmount);
        for (int i = 0; i < pelletAmount; i++)
        {
            DoRaycastHit(ammo, directions[i], out RaycastHit hit);
            StartCoroutine(SpawnTrail(hit, directions[i]));
        }
    }

    private void OnDrawGizmos()
    {
        if (!activeGun) return;
        if (headTransform == null) return;
        float pelletSpread = Mathf.Max(CurrentStats.horizontalSpread, CurrentStats.verticalSpread);
        Vector3[] conePoints = ConeDrawer.GetConePointsAngle(headTransform, pelletSpread, pelletAmount);
        for (int i = 0; i < pelletAmount; i++)
        {
            RaycastHit hit;

            Vector3 headPosition = headTransform.position;

            float range = CurrentStats.maximumEffectiveRange;
            if (Physics.Raycast(headPosition, conePoints[i], out hit, range))
            {
                if (hit.collider.TryGetComponent(out IDamageable damageable))
                    Gizmos.color = Color.red;
                else
                    Gizmos.color = Color.white;
                Gizmos.DrawLine(headTransform.position, hit.point);
            }
            else
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(headTransform.position, headTransform.position + conePoints[i] * range);
            }
        }
    }
}

