using UnityEngine;

public class EnemyHealth : Health
{
    public override void Damage(float damage)
    {
        base.Damage(damage);
        Debug.Log($"{gameObject.name} damaged for: {damage}");
    }
}
