using UnityEngine;

public class HitCollision : MonoBehaviour
{
    public Health health;
    public float velocityAmount = 2f;
    [SerializeField] bool weakSpot;

    private void OnValidate()
    {
        if (TryGetComponent(out DrawCollider drawer)) drawer.color = weakSpot == true ? new (1f, 0f, 0f) : new(1, 0.5f, 0f);
    }

    public void GetHit(Transform inflictor)
    {
        if (!health.IsDead) return;
        if (!transform.parent.TryGetComponent(out Rigidbody rb)) return;

        Vector3 position = transform.position;
        Vector3 angle = Vector3.RotateTowards(inflictor.forward, inflictor.position - position, 360, 360);
        rb.velocity -= angle.normalized * velocityAmount;
    }

    public static bool CheckHitCollision(Collider collider, Transform inflictor, out bool weakPointHit, out Health health)
    {
        weakPointHit = false;
        if (collider.TryGetComponent(out health)) return true;
        else if (collider.TryGetComponent(out HitCollision hitCollider))
        {
            weakPointHit = hitCollider.weakSpot;
            hitCollider.GetHit(inflictor);
            health = hitCollider.health;
            bool returnValue = health != null ? true : false;
            return returnValue;
        }
        return false;
    }
}
