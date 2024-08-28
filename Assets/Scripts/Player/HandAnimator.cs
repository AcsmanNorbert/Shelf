using UnityEngine;

public class HandAnimator : MonoBehaviour
{
    [SerializeField] PlayerMovement mover;
    [SerializeField] Transform target;
    [SerializeField] float verticalAmount;
    [SerializeField] float horizontalAmount;
    [SerializeField] float verticalSpeed;
    [SerializeField] float horizontalSpeed;

    private void Update()
    {
        Vector3 verticalVelocity = new Vector3(0, mover.Velocity.y * verticalAmount, 0);
        Vector3 horizontalVelocity = mover.InputVelocity() * horizontalAmount;
        target.position = Vector3.Lerp(target.position, transform.position + verticalVelocity, Time.deltaTime * verticalSpeed);
        target.position = Vector3.Lerp(target.position, transform.position + horizontalVelocity, Time.deltaTime * horizontalSpeed);
    }
}
