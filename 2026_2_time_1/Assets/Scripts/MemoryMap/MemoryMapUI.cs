using UnityEngine;
using UnityEngine.InputSystem;


public class MemoryMapUI : MonoBehaviour
{
    public static MemoryMapUI Current { get; private set; }
    public static bool IsOpen => Current != null && (Current.panel.activeSelf || Current.closedFrame == Time.frameCount);

    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private GameObject panel;
    [SerializeField] private MemoryMapBoard board;

    private InputActionMap playerMap;
    private InputAction toggleAction;
    private InputAction cancelAction;
    private int closedFrame = -1;

    private void Awake()
    {
        playerMap = inputActions.FindActionMap("Player", throwIfNotFound: true);
        toggleAction = playerMap.FindAction("ToggleMemoryMap", throwIfNotFound: true);
        cancelAction = playerMap.FindAction("Cancel", throwIfNotFound: true);
        if (board == null)
            board = GetComponentInChildren<MemoryMapBoard>(true);
        panel.SetActive(false);
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
        if (toggleAction.WasPressedThisFrame())
            SetOpen(!panel.activeSelf);
        else if (panel.activeSelf && cancelAction.WasPressedThisFrame())
        {
            bool rightClick = Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame;
            if (rightClick)
            {
                if (board != null)
                    board.ZoomOut();
            }
            else
            {
                SetOpen(false);
            }
        }
    }

    public void SetOpen(bool open)
    {
        if (!open && panel.activeSelf)
            closedFrame = Time.frameCount;
        bool opening = open && !panel.activeSelf;
        panel.SetActive(open);
        if (opening && board != null)
            board.ShowCurrentArea();
    }
}
