using Unity.VisualScripting;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [SerializeField] float amount;
    [SerializeField] Collider myCollider;
    [SerializeField] bool showGizmos = true;

    private void OnTriggerStay(Collider collider)
    {
        GameObject target = collider.gameObject;

        if (target.TryGetComponent(out PlayerMovement mover))
        {
            if (mover.Velocity.y <= 0.2f) mover.AddVelocity(target.transform.up * amount);
            return;
        }

        if (target.TryGetComponent(out Rigidbody rb))
        {
            if (rb.velocity.y <= 0.2f) rb.velocity += target.transform.up * amount;
            return;
        }
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos) return;

        float gravity = Physics.gravity.y;
        PlayerMovement mover = FindObjectOfType<PlayerMovement>();
        if (mover != null) gravity = mover.Gravity;
        gravity = Mathf.Abs(gravity);

        Gizmos.color = Color.green;
        Vector3 jumpHeight = transform.up * gravity * Mathf.Pow(amount, 2);
        float mult = 0.003f;
        Gizmos.DrawLine(transform.position, transform.position + jumpHeight * mult);
    }
}
