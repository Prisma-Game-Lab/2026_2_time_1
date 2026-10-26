using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InspectionManager : MonoBehaviour
{
    public static InspectionManager Current { get; private set; }
    public static bool IsOpen => Current != null && Current.CurrentView != null;

    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private Color dimColor = new Color(0f, 0f, 0f, 0.75f);
    [SerializeField] private int sortingOrder = 5;
    [SerializeField] private Vector2 referenceResolution = new Vector2(1920f, 1080f);

    private InputActionMap playerMap;
    private InputAction cancelAction;

    private Canvas overlay;
    private RectTransform contentRoot;

    public InspectView CurrentView { get; private set; }

    public event Action<InspectView> OnOpened;
    public event Action<InspectView> OnClosed;

    private void Awake()
    {
        playerMap = inputActions.FindActionMap("Player", throwIfNotFound: true);
        cancelAction = playerMap.FindAction("Cancel", throwIfNotFound: true);

        CreateOverlay();
    }

    private void OnEnable()
    {
        Current = this;
        playerMap.Enable();
    }

    // An open view is kept as it is, so a frozen scene comes back still inspecting it
    private void OnDisable()
    {
        if (Current == this)
            Current = null;
        playerMap.Disable();
    }

    private void Update()
    {
        if (CurrentView != null && cancelAction.WasPressedThisFrame())
            Close();
    }

    public void Open(InspectView viewPrefab, WorldItem source)
    {
        if (viewPrefab == null)
            return;

        Close();

        // A nested Canvas keeps the size it was saved with in the prefab (usually a scale of 0), so it never shows up
        if (viewPrefab.GetComponentInChildren<Canvas>(true) != null)
            Debug.LogWarning($"[InspectionManager] '{viewPrefab.name}' has its own Canvas and probably won't show. Remove it, the overlay already is the canvas.");

        overlay.gameObject.SetActive(true);
        CurrentView = Instantiate(viewPrefab, contentRoot, false);
        CurrentView.Init(source);
        OnOpened?.Invoke(CurrentView);
    }

    public void Close()
    {
        if (CurrentView == null)
            return;

        InspectView closed = CurrentView;
        CurrentView = null;
        overlay.gameObject.SetActive(false);
        closed.NotifyClosed();
        Destroy(closed.gameObject);
        OnClosed?.Invoke(closed);
    }

    private void CreateOverlay()
    {
        GameObject overlayObject = new GameObject("InspectionOverlay", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        overlayObject.transform.SetParent(transform, false);

        overlay = overlayObject.GetComponent<Canvas>();
        overlay.renderMode = RenderMode.ScreenSpaceOverlay;
        overlay.sortingOrder = sortingOrder;

        CanvasScaler scaler = overlayObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = referenceResolution;
        scaler.matchWidthOrHeight = 0.5f;

        // Clicking the background (anywhere outside the view) closes it
        Image dim = CreateFullScreen("Dim", overlayObject.transform).gameObject.AddComponent<Image>();
        dim.color = dimColor;
        Button dimButton = dim.gameObject.AddComponent<Button>();
        dimButton.transition = Selectable.Transition.None;
        dimButton.navigation = new Navigation { mode = Navigation.Mode.None };
        dimButton.onClick.AddListener(Close);

        contentRoot = CreateFullScreen("Content", overlayObject.transform);

        overlayObject.SetActive(false);
    }

    private static RectTransform CreateFullScreen(string name, Transform parent)
    {
        RectTransform rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        return rect;
    }
}
