using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] PlayerInput playerInput;
    [SerializeField] float movementSpeed = 10f;

    [Space(3)]
    [Header("Jump")]
    [SerializeField] Transform groundCheckTransform;
    [SerializeField] Transform headCheckTransform;
    [SerializeField] LayerMask playerMask;
    public LayerMask PlayerMask => playerMask;
    [SerializeField] float gravity = -9.81f; // originaly -9.81f
    public float Gravity { get => gravity; }
    [SerializeField] float jumpHeight = 3f;
    [SerializeField] int maxJumpAmount = 2;

    CharacterController controller;

    public static Transform GetHeadTransform() => Camera.main.transform;

    private void OnValidate()
    {
        if (playerInput == null) playerInput = GetComponent<PlayerInput>();
    }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerInput.OnJumpKey += Jump;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        availableJumps = maxJumpAmount;
    }

    private void Update()
    {
        JumpUpdate();
        MovementUpdate();
    }

    int availableJumps;
    bool grounded = true;
    Vector3 velocity;
    public Vector3 Velocity { get => velocity + InputVelocity(); }

    float forwardSpeed;
    float horizontalSpeed;
    public Vector3 InputVelocity() => transform.right * horizontalSpeed + transform.forward * forwardSpeed;
    public Vector3 InputMovement() => new(horizontalSpeed, 0, forwardSpeed);


    [SerializeField] float checkRange = 0.2f;

    void JumpUpdate()
    {
        grounded = Physics.CheckSphere(groundCheckTransform.position, checkRange, ~playerMask);
        if (grounded && velocity.y < 0)
        {
            // this is set so it does not register grounded when you start a jump
            velocity.y = -2f;
            availableJumps = maxJumpAmount;
        }

        if (Physics.CheckSphere(headCheckTransform.position, checkRange, ~playerMask) && velocity.y > 0) velocity.y = 0f;
    }

    public void Jump()
    {
        if (availableJumps <= 0) return;

        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        availableJumps--;
        PlayEffectsSelf(WeaponEvent.Jump);
    }

    void MovementUpdate()
    {
        horizontalSpeed = playerInput.HorizontalInput;
        forwardSpeed = playerInput.VerticalInput;

        Vector3 inputMovemnt = InputVelocity();

        controller.Move(inputMovemnt * movementSpeed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    public void AddVelocity(Vector3 velocity) => this.velocity += velocity;

    protected void PlayEffectsSelf(WeaponEvent weaponEvent)
    {
        foreach (IWeaponEffect player in GetComponents<IWeaponEffect>())
            player.DoEffect(weaponEvent);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheckTransform.position, checkRange);
        Gizmos.DrawWireSphere(headCheckTransform.position, checkRange);
    }
}
