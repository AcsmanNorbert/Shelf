using UnityEngine;

public class Billboarding : MonoBehaviour
{
    void Update()
    {
        transform.rotation = Quaternion.Euler(
            Camera.main.transform.rotation.eulerAngles.x, 
            Camera.main.transform.rotation.eulerAngles.y,
            0f);
    }
}
