using UnityEngine;

public class Recoil : MonoBehaviour
{   
    float snappiness = 0f;
    float returnSpeed = 0f;

    Vector3 currentRotation;
    Vector3 targetRotation;

    void Update()
    {
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(currentRotation);
    }

    public void RecoilFire(float recoilX, float recoilY, float snappiness, float returnSpeed)
    {
        targetRotation += new Vector3(recoilX, Random.Range(-recoilY, recoilY));
        this.snappiness = snappiness;
        this.returnSpeed = returnSpeed;
    }
}
