using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private float rotationAngle = 90f;
    [SerializeField] private float rotationDuration = 0.25f;

    private InputActionMap playerMap;
    private InputAction rotateLeftAction;
    private InputAction rotateRightAction;

    private Camera currentCamera;
    private float currentYaw;
    private float targetYaw;

    public bool IsRotating => !Mathf.Approximately(currentYaw, targetYaw);

    private void Awake()
    {
        playerMap = inputActions.FindActionMap("Player", throwIfNotFound: true);
        rotateLeftAction = playerMap.FindAction("RotateLeft", throwIfNotFound: true);
        rotateRightAction = playerMap.FindAction("RotateRight", throwIfNotFound: true);
    }

    private void OnEnable()
    {
        playerMap.Enable();
    }

    private void OnDisable()
    {
        playerMap.Disable();
    }

    private void Update()
    {
        Camera cam = Camera.main;
        if (cam == null)
            return;

        if (cam != currentCamera)
        {
            currentCamera = cam;
            currentYaw = targetYaw = cam.transform.eulerAngles.y;
        }

        bool isTransitioning = GameManager.Instance != null && GameManager.Instance.IsTransitioning;
        if (!isTransitioning && !InspectionManager.IsOpen)
        {
            if (rotateLeftAction.WasPressedThisFrame())
                targetYaw -= rotationAngle;
            if (rotateRightAction.WasPressedThisFrame())
                targetYaw += rotationAngle;
        }

        if (!IsRotating)
            return;

        if (rotationDuration <= 0f)
            currentYaw = targetYaw;
        else
            currentYaw = Mathf.MoveTowards(currentYaw, targetYaw, rotationAngle / rotationDuration * Time.deltaTime);

        if (!IsRotating)
            currentYaw = targetYaw = Mathf.Repeat(targetYaw, 360f);

        Vector3 euler = cam.transform.eulerAngles;
        euler.y = currentYaw;
        cam.transform.eulerAngles = euler;
    }
}
