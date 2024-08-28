using UnityEngine;

public class DrawCollider : MonoBehaviour
{
    public Color color = Color.red;
    public bool drawCollider = true;

    private void OnDrawGizmos()
    {
        if (!drawCollider) return;
        if (TryGetComponent(out Collider collider))
        {
            Gizmos.color = color;
            BoxCollider boxCollider = GetComponent<BoxCollider>();
            if (boxCollider != null)
            {
                Vector3 center = new Vector3(
                    boxCollider.center.x * transform.lossyScale.x,
                    boxCollider.center.y * transform.lossyScale.y,
                    boxCollider.center.z * transform.lossyScale.z);
                Matrix4x4 gizmoMatrix = Matrix4x4.TRS(transform.position + center, transform.rotation, transform.lossyScale);
                Gizmos.matrix = gizmoMatrix;
                Gizmos.DrawWireCube(Vector3.zero, boxCollider.size);
                return;
            }

            SphereCollider sphereCollider = GetComponent<SphereCollider>();
            if (sphereCollider != null)
            {
                float biggestSide = CheckBiggestSideOfVector3(transform.lossyScale);
                Vector3 scale = new(biggestSide, biggestSide, biggestSide);
                Vector3 center = new Vector3(
                    sphereCollider.center.x * transform.lossyScale.x,
                    sphereCollider.center.y * transform.lossyScale.y,
                    sphereCollider.center.z * transform.lossyScale.z);
                Matrix4x4 gizmoMatrix = Matrix4x4.TRS(transform.position + center, transform.rotation, scale);
                Gizmos.matrix = gizmoMatrix;
                Gizmos.DrawWireSphere(Vector3.zero, sphereCollider.radius);
                return;
            }

            CapsuleCollider capsuleCollider = GetComponent<CapsuleCollider>();
            if(capsuleCollider != null)
            {
                float biggestSide = CheckBiggestSideOfVector3(transform.lossyScale);
                Vector3 scale = new(biggestSide, biggestSide, biggestSide);
                Vector3 center = new Vector3(
                    capsuleCollider.center.x * transform.lossyScale.x,
                    capsuleCollider.center.y * transform.lossyScale.y,
                    capsuleCollider.center.z * transform.lossyScale.z);

                Quaternion rotation = transform.rotation;
                Vector3 centerTransform = transform.position + center;

                float capsuleRadius = capsuleCollider.radius;
                float capsuleHeight = capsuleCollider.height / 2;
                float sphereHeight = capsuleHeight - capsuleRadius;

                Vector3 sphereTopUpper = transform.up * sphereHeight * biggestSide;
                Matrix4x4 gizmoMatrix = Matrix4x4.TRS(centerTransform + sphereTopUpper, rotation, scale);
                Gizmos.matrix = gizmoMatrix;
                Gizmos.DrawWireSphere(Vector3.zero, capsuleRadius);

                Vector3 sphereTopLower = -transform.up * sphereHeight * biggestSide;
                gizmoMatrix = Matrix4x4.TRS(centerTransform + sphereTopLower, rotation, scale);
                Gizmos.matrix = gizmoMatrix;
                Gizmos.DrawWireSphere(Vector3.zero, capsuleRadius);

                Vector3 forwardOffset = center + transform.forward * capsuleRadius;
                Vector3 rightOffset= center + transform.right * capsuleRadius;
                Vector3 upOffset = transform.up * sphereHeight;
                gizmoMatrix = Matrix4x4.TRS(centerTransform, Quaternion.identity, scale);
                Gizmos.matrix = gizmoMatrix;
                Gizmos.DrawLine(forwardOffset + upOffset, forwardOffset - upOffset);
                Gizmos.DrawLine(-forwardOffset + upOffset, -forwardOffset - upOffset);
                Gizmos.DrawLine(rightOffset + upOffset, rightOffset - upOffset);
                Gizmos.DrawLine(-rightOffset + upOffset, -rightOffset - upOffset);

                return;
            }
        }
    }

    private float CheckBiggestSideOfVector3(Vector3 vector)
    {
        float biggest = vector.x;
        if (biggest < vector.y) biggest = vector.y;
        if (biggest < vector.z) biggest = vector.z;
        return biggest;
    }
}
