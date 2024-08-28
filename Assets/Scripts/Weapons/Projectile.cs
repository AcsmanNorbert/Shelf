using System.Collections;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [SerializeField] bool drawGizmos;
    [SerializeField] float radius;
    float maxDistance;
    DamageData data;
    Rigidbody rb;
    Vector3 startPosition;
    bool gettingDestroyed;
    LayerMask targetMask;

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        startPosition = transform.position;
    }

    public void SetupProjectile(DamageData data, float speed, float maxDistance)
    {
        this.data = data;
        this.maxDistance = maxDistance;
        targetMask = ~data.inflicter.ReverseTargetMask;
        rb.velocity += transform.forward * speed;
    }


    private void Update()
    {
        if (gettingDestroyed) return;

        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, targetMask);

        if (Vector3.Distance(startPosition, transform.position) >= maxDistance) DestroySelf();

        if (colliders.Length != 0)
        {
            foreach (Collider collider in colliders)
            {
                if (HitCollision.CheckHitCollision(collider, data.inflicter.transform, out bool weakPointHit, out Health health))
                {
                    health.Damage(data, weakPointHit);
                    break;
                }
            }
            DestroySelf();
        }
    }

    void DestroySelf()
    {
        gettingDestroyed = true;
        rb.velocity = Vector3.zero;
        float killTime = 0f;

        TrailRenderer[] trailRenderers = GetComponentsInChildren<TrailRenderer>();
        if (trailRenderers != null)
        {
            killTime = trailRenderers[0].time;
            for (int i = 1; i < trailRenderers.Count(); i++)
                killTime = Mathf.Max(killTime, trailRenderers[i].time);
        }

        StartCoroutine(LerpLight(killTime));

        Destroy(gameObject, killTime);
    }

    IEnumerator LerpLight(float killTime)
    {
        Light[] lights = GetComponentsInChildren<Light>();
        float timer = 0f;
        while (timer < killTime)
        {
            foreach (Light light in lights)
                light.range = Mathf.Lerp(light.range, 0, timer / killTime);
            timer += Time.deltaTime;
            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos || gettingDestroyed) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
