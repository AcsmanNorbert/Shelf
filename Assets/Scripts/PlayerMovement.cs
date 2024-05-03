using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float movementSpeed = 10f;
    [SerializeField] float mouseSensitivity = 2f;
    [SerializeField] float jumpHeight = 8f;
    [SerializeField] GameObject playerCamera;

    Rigidbody rb;

    float forwardSpeed;
    float rightSpeed;
    float cameraRotationY;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        forwardSpeed = Input.GetAxis("Horizontal");
        rightSpeed = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.Space))
            rb.velocity = new(rb.velocity.x, jumpHeight, rb.velocity.z);

        //rotation
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        cameraRotationY -= mouseY;
        cameraRotationY = Mathf.Clamp(cameraRotationY, -90f, 90f);
        playerCamera.transform.localEulerAngles = Vector3.right * cameraRotationY;

        transform.Rotate(Vector3.up * mouseX);
    }

    private void FixedUpdate()
    {
        //movement
        Vector3 inputMovement = new(forwardSpeed, 0, rightSpeed);
        //rotates the current input based on camera location
        inputMovement = Quaternion.Euler(Camera.main.gameObject.transform.eulerAngles.x, Camera.main.gameObject.transform.eulerAngles.y, 0f) * inputMovement;
        //magnifies the movement by its speed multiplyer and makes Y unchanged
        inputMovement = new(
            inputMovement.x * movementSpeed * Time.fixedDeltaTime,
            rb.velocity.y,
            inputMovement.z * movementSpeed * Time.fixedDeltaTime);
        rb.velocity = inputMovement;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 start = playerCamera.transform.position;
        Vector3 end = playerCamera.transform.position + playerCamera.transform.forward * 10;
        Gizmos.DrawLine(start, end);
    }

    public static Transform GetHeadTransform() => Camera.main.transform;
}
