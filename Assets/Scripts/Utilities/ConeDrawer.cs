using UnityEngine;

public class ConeDrawer : MonoBehaviour
{
    [SerializeField] int pelletCount;
    [SerializeField] float angle;
    [SerializeField] Gradient gradient;
    [SerializeField] float range = 5;
    [SerializeField] float offset = 0.01f;

    Vector3[] randomOffset;

    private void OnValidate()
    {
        randomOffset = new Vector3[pelletCount];
        for (int i = 0; i < pelletCount; i++)
            randomOffset[i] = new(Random.Range(-offset, offset), Random.Range(-offset, offset));
    }

    public static Vector3[] GetConePointsAngle(Transform transform, float angle, int pointCount)
    {
        Vector3[] points = new Vector3[pointCount];
        float deltaAngle = 2 * Mathf.PI / pointCount;
        for (int i = 0; i < pointCount; i++)
        {
            Vector3 forwardAngle = transform.forward;

            float a = Mathf.Sin(angle * Mathf.Deg2Rad);
            float b = Mathf.Cos(angle * Mathf.Deg2Rad);
            float rotationAangleA = Mathf.Sin(deltaAngle * i);
            float rotationAangleB = Mathf.Cos(deltaAngle * i);

            Vector3 newAngle = transform.forward * b +
                transform.up * a * rotationAangleB +
                transform.right * a * rotationAangleA;
            points[i] = newAngle;
        }

        return points;
    }
    public static Vector3[] GetConePointsAngleSpread(Transform transform, float minAngle, float maxAngle, int pointCount)
    {
        Vector3[] points = new Vector3[pointCount];
        float deltaAngle = 2 * Mathf.PI / pointCount;
        for (int i = 0; i < pointCount; i++)
        {
            Vector3 forwardAngle = transform.forward;

            float angle = Random.Range(minAngle, maxAngle);

            float a = Mathf.Sin(angle * Mathf.Deg2Rad);
            float b = Mathf.Cos(angle * Mathf.Deg2Rad);
            float rotationAangleA = Mathf.Sin(deltaAngle * i);
            float rotationAangleB = Mathf.Cos(deltaAngle * i);

            Vector3 newAngle = transform.forward * b +
                transform.up * a * rotationAangleB +
                transform.right * a * rotationAangleA;
            points[i] = newAngle;
        }

        return points;
    }

    private void OnDrawGizmos()
    {
        Vector3[] points = GetConePointsAngle(transform, angle, pelletCount);

        for (int i = 0; i < pelletCount; i++)
        {
            float colorTime = i / (float)pelletCount;
            Gizmos.color = gradient.Evaluate(colorTime);
            Gizmos.DrawLine(transform.position, points[i] + randomOffset[i] * range);
        }
    }
}
