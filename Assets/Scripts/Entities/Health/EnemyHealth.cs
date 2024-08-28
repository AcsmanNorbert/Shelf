using UnityEngine;

public class EnemyHealth : Health
{
    [SerializeField] EnemyNavMesh navMesh;
    public bool disolveAfterDeath;

    public override void Damage(DamageData data, bool weakPointHit)
    {
        base.Damage(data, weakPointHit);
        if (!isDead && navMesh != null) navMesh.TargetFoundAlert();

    }

    public static float corpseTimer = 8f;
    public override void Death()
    {
        base.Death();
        if (navMesh != null) navMesh.SetState(EnemyNavMesh.State.Dead);
        if (disolveAfterDeath) Destroy(gameObject, corpseTimer);
    }
}
