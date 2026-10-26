using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PointAndClick : MonoBehaviour
{
    public static PointAndClick Current { get; private set; }

    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private LayerMask interactableLayers = ~0;

    private InputActionMap playerMap;
    private InputAction clickAction;
    private InputAction pointAction;

    private void Awake()
    {
        playerMap = inputActions.FindActionMap("Player", throwIfNotFound: true);
        clickAction = playerMap.FindAction("Click", throwIfNotFound: true);
        pointAction = playerMap.FindAction("Point", throwIfNotFound: true);
    }

    private void OnEnable()
    {
        Current = this;
        playerMap.Enable();
    }

    private void OnDisable()
    {
        if (Current == this)
            Current = null;
        playerMap.Disable();
    }

    private void Update()
    {
        if (!clickAction.WasPressedThisFrame())
            return;

        if (GameManager.Instance != null && GameManager.Instance.IsTransitioning)
            return;

        if (InspectionManager.IsOpen)
            return;

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        WorldItem worldItem = GetItemAtScreenPoint(pointAction.ReadValue<Vector2>());
        if (worldItem != null)
            worldItem.Interact();
    }

    public WorldItem GetItemAtScreenPoint(Vector2 screenPoint)
    {
        Camera cam = Camera.main;
        if (cam == null)
            return null;

        return GetTopmostItemAlong(cam.ScreenPointToRay(screenPoint), cam.farClipPlane);
    }

    private WorldItem GetTopmostItemAlong(Ray ray, float maxDistance)
    {
        WorldItem topmost = null;
        int topLayer = int.MinValue;
        int topOrder = int.MinValue;
        float topDistance = float.MaxValue;

        foreach (RaycastHit hit in Physics.RaycastAll(ray, maxDistance, interactableLayers, QueryTriggerInteraction.Collide))
        {
            WorldItem worldItem = hit.collider.GetComponentInParent<WorldItem>();
            if (worldItem == null)
                continue;

            int layer = SortingLayer.GetLayerValueFromID(worldItem.SpriteRenderer.sortingLayerID);
            int order = worldItem.SpriteRenderer.sortingOrder;
            if (layer > topLayer
                || (layer == topLayer && order > topOrder)
                || (layer == topLayer && order == topOrder && hit.distance < topDistance))
            {
                topmost = worldItem;
                topLayer = layer;
                topOrder = order;
                topDistance = hit.distance;
            }
        }

        return topmost;
    }
}
