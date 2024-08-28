using UnityEngine;

public class AutoRotate : MonoBehaviour
{
    enum Axes
    {
        X, Y, Z
    }

    [SerializeField] Axes axes;
    [SerializeField] float speed;

    private void Update()
    {
        Vector3 eulerAngles = transform.eulerAngles;
        Vector3 rotationVector = new Vector3(0,0,0);
        switch (axes)
        {
            case Axes.X: 
                rotationVector = new Vector3(eulerAngles.x + speed * Time.deltaTime, 0, 0);
                break;
            case Axes.Y:
                rotationVector = new Vector3(0, eulerAngles.y + speed * Time.deltaTime, 0);
                break;
            case Axes.Z:
                rotationVector = new Vector3(0, 0, eulerAngles.z + speed * Time.deltaTime);
                break;
            default:
                break;
        }
        transform.eulerAngles = rotationVector;
    }
}
