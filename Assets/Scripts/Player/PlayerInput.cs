using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] LoadoutManager loadoutManager;

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Alpha1)) loadoutManager.SwapWeapon(0);
        //if (Input.GetKeyDown(KeyCode.Alpha2)) loadoutManager.SwapWeapon(1);
        //if (Input.GetKeyDown(KeyCode.Alpha3)) loadoutManager.SwapWeapon(2);

        //if (Input.GetKeyDown(KeyCode.R)) loadoutManager.CurrentGun.Reload();
        //if (Input.GetMouseButtonUp(1)) loadoutManager.CurrentGun.ZoomOut();
        //if (Input.GetMouseButtonDown(1)) loadoutManager.CurrentGun.ZoomIn();

        if (weaponFire1) OnWeaponFire?.Invoke();
        if (reload) OnReload?.Invoke();
    }

    private void FixedUpdate()
    {
        smoothMovementInput = Vector2.SmoothDamp(smoothMovementInput, movementInput, ref movementVelocity, movementSmoothness);
    }

    bool IsButtonPerformed(InputActionPhase phase)
    {
        switch (phase)
        {
            case InputActionPhase.Performed: return true;
            case InputActionPhase.Canceled: return false;
            default: return false;
        }
    }

    #region MOVEMENT
    private Vector2 movementInput;
    private Vector2 smoothMovementInput;
    private Vector2 movementVelocity;
    [SerializeField] float movementSmoothness = 0.1f;
    public void Move(InputAction.CallbackContext context) => movementInput = context.ReadValue<Vector2>();
    public float MouseX => cameraMovement.x; //Input.GetAxis("Mouse X");
    public float MouseY => cameraMovement.y; //Input.GetAxis("Mouse Y");

    public void Jump(InputAction.CallbackContext context) { if (context.phase == InputActionPhase.Performed) OnJumpKey?.Invoke(); }
    public Action OnJumpKey;
    #endregion

    #region CAMERA
    private Vector2 cameraMovement;
    public void CameraRotation(InputAction.CallbackContext context) => cameraMovement = context.ReadValue<Vector2>();
    public float HorizontalInput => smoothMovementInput.x; //Input.GetAxis("Horizontal");
    public float VerticalInput => smoothMovementInput.y; //Input.GetAxis("Vertical");
    #endregion

    #region WEAPON
    bool weaponFire1;
    public void WeaponFire(InputAction.CallbackContext context) => weaponFire1 = IsButtonPerformed(context.phase);
    public Action OnWeaponFire;

    public void Zoom(InputAction.CallbackContext context) => OnZoom?.Invoke(IsButtonPerformed(context.phase));       
    public Action<bool> OnZoom;

    bool reload;
    public void Reload(InputAction.CallbackContext context) => reload = IsButtonPerformed(context.phase);
    public Action OnReload;
    #endregion

    #region LOADOUT
    public void Slot1() => OnSwapWeapon?.Invoke(0);
    public void Slot2() => OnSwapWeapon?.Invoke(1);
    public void Slot3() => OnSwapWeapon?.Invoke(2);
    public Action<int> OnSwapWeapon;

    public void NextWeapon() => OnNextWeapon?.Invoke(1);
    public void PreviousWeapon() => OnNextWeapon?.Invoke(-1);
    /// <summary>
    /// Swaps weapons (int == slot number 0-2)
    /// </summary>
    /// <summary>
    /// Swaps to next weapon if "+1", to previous if "-1"
    /// </summary>
    public Action<int> OnNextWeapon;
    #endregion
}
