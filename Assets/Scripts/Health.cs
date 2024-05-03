using UnityEngine;

public abstract class Health : MonoBehaviour
{
    public float startingHealth = 10;
    protected float health;

    private void Start()
    {
        health = startingHealth;
    }

    public virtual void Damage(float damage)
    {
        health -= damage;
        if (health <= 0)
            Destroy(gameObject);
    }
}