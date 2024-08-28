using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] PlayerInput playerInput;
    [SerializeField] Transform head;
    [SerializeField] Camera mainCamera;
    [SerializeField] Camera fpsCamera;
    [SerializeField] Recoil recoil;
    [SerializeField] ScopeController scopeController;
    public Recoil Recoil => recoil;

    [SerializeField] float mouseSensitivity = 2f;
    [SerializeField] float zoomSensitivity = 1.5f;

    [SerializeField] float zoomTime;
    [SerializeField] float fpsZoomAmount;
    float baseFOW;

    float baseZoom;
    float fpsZoom;
    float baseZoomIn;
    float fpsZoomIn;
    bool zoomedIn;

    private void OnValidate()
    {
        if (playerInput == null) playerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        baseFOW = mainCamera.fieldOfView;
        baseZoomIn = baseFOW;
        baseZoom = baseFOW;
        fpsZoomIn = baseFOW;
        fpsZoom = baseFOW;
    }

    public bool ZoomIn(float zoom, Sprite scopeCrosshair, float scopeSize, float readyTime)
    {
        zoom = Mathf.Clamp(zoom, 1, baseFOW * 0.9f);
        baseZoom = zoom;
        fpsZoom = zoom * fpsZoomAmount;
        zoomedIn = true;

        if (scopeCrosshair != null)
        {
            scopeController.ScopeIn(scopeCrosshair, scopeSize, readyTime);
            fpsCamera.enabled = false;
        }

        return true;
    }

    public bool ZoomOut()
    {
        baseZoom = baseFOW;
        fpsZoom = baseFOW;
        zoomedIn = false;

        scopeController.ScopeOut();

        fpsCamera.enabled = true;

        return false;
    }

    private void Update()
    {
        ZoomUpdate();
        RotationUpdate();
    }

    void ZoomUpdate()
    {
        baseZoomIn = Mathf.Lerp(baseZoomIn, baseZoom, Time.deltaTime * zoomTime);
        fpsZoomIn = Mathf.Lerp(fpsZoomIn, fpsZoom, Time.deltaTime * zoomTime);
        mainCamera.fieldOfView = baseZoomIn;
        fpsCamera.fieldOfView = fpsZoomIn;
    }

    float cameraRotationX;
    float cameraRotationY;

    void RotationUpdate()
    {
        float currentSesitivity = zoomedIn ? zoomSensitivity : mouseSensitivity;
        float mouseX = playerInput.MouseX * currentSesitivity;
        float mouseY = playerInput.MouseY * currentSesitivity;

        cameraRotationX = mouseX;

        cameraRotationY -= mouseY;
        cameraRotationY = Mathf.Clamp(cameraRotationY, -90f, 90f);
        head.localEulerAngles = Vector3.right * cameraRotationY;

        transform.Rotate(Vector3.up * cameraRotationX);
    }

    public float ZoomAmount() => baseZoomIn / baseFOW;
}
