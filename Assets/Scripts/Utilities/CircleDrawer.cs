using UnityEngine;

public class CircleDrawer : MonoBehaviour
{
    public static Vector3[] GetPositions(Transform transform, float radius, int pointCount, bool closeCircle)
    {
        int count = closeCircle ? pointCount + 1 : pointCount;
        Vector3[] result = new Vector3[count];
        float deltaAngle = 2 * Mathf.PI / pointCount ;
        Vector3 center = transform.position;

        for (int i = 0; i < count; i++)
        {
            float angle = (i % count) * deltaAngle;
            result[i] = GetPoint(angle, radius, center);
        }

        return result;
    }

    static Vector3 GetPoint(float angle, float radius, Vector3 center)
    {
        float x = Mathf.Cos(angle);
        float y = Mathf.Sin(angle);

        return (new Vector3(x, y) * radius) + center;
    }
}
