using UnityEngine;

public class RotateTowards : MonoBehaviour
{
    [SerializeField] Transform target;
    // Update is called once per frame
    void Update()
    {
        transform.LookAt(target.position);
    }
}
