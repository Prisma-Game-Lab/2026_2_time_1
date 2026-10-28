using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class Inventory : MonoBehaviour
{
    public const int MaxSlots = 5;

    public static Inventory Current { get; private set; }

    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private Transform slotsPanel;
    [SerializeField] private Image[] slotIcons = new Image[MaxSlots];
    [SerializeField] private bool startOpen = true;
    [SerializeField] private bool keepItemsBetweenVisits = false;
    private string dataKey;
    private CollectibleItem[] slots;
    private HashSet<CollectibleItem> collected;

    private Color[] slotColors;

    private InputActionMap playerMap;
    private InputAction toggleAction;

    private Image dragIcon;
    private RectTransform dragCanvas;
    private int draggingIndex = -1;

    public event Action OnInventoryChanged;

    public IReadOnlyList<CollectibleItem> Items => slots;
    public bool IsFull => FindEmptySlot() < 0;
    public bool IsOpen => slotsPanel != null && slotsPanel.gameObject.activeSelf;
    public bool IsMemoryInventory => InventoryData.IsMemoryKey(dataKey);

    private void Awake()
    {
        dataKey = InventoryData.KeyFor(gameObject.scene);
        InventoryData.Container data = InventoryData.Get(dataKey);
        slots = data.Slots;
        collected = data.Collected;

        playerMap = inputActions.FindActionMap("Player", throwIfNotFound: true);
        toggleAction = playerMap.FindAction("ToggleInventory", throwIfNotFound: true);

        if (slotsPanel != null && (slotIcons == null || Array.TrueForAll(slotIcons, icon => icon == null)))
        {
            slotIcons = new Image[Mathf.Min(MaxSlots, slotsPanel.childCount)];
            for (int i = 0; i < slotIcons.Length; i++)
            {
                Transform slot = slotsPanel.GetChild(i);
                slotIcons[i] = slot.childCount > 0 ? slot.GetChild(0).GetComponent<Image>() : slot.GetComponent<Image>();
            }
        }

        slotColors = new Color[slotIcons.Length];
        for (int i = 0; i < slotIcons.Length; i++)
        {
            if (slotIcons[i] == null)
                continue;

            slotColors[i] = slotIcons[i].color;

            if (!slotIcons[i].TryGetComponent(out InventorySlot slot))
                slot = slotIcons[i].gameObject.AddComponent<InventorySlot>();
            slot.Init(this, i);
        }

        CreateDragIcon();
        RefreshUI();
        SetOpen(startOpen);
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

        CancelDrag();
    }
    private void OnDestroy()
    {
        if (IsMemoryInventory && !keepItemsBetweenVisits)
            InventoryData.Clear(dataKey);
    }

    private void Update()
    {
        if (toggleAction.WasPressedThisFrame())
            SetOpen(!IsOpen);
    }

    public void SetOpen(bool open)
    {
        if (slotsPanel == null)
            return;

        if (!open)
            CancelDrag();
        slotsPanel.gameObject.SetActive(open);
    }

    public bool AddItem(CollectibleItem item)
    {
        int index = FindEmptySlot();
        if (item == null || index < 0 || HasItem(item))
            return false;

        slots[index] = item;
        collected.Add(item);
        Changed();
        return true;
    }

    public bool RemoveItem(CollectibleItem item)
    {
        return item != null && RemoveAt(Array.IndexOf(slots, item));
    }

    public bool RemoveAt(int index)
    {
        if (GetItem(index) == null)
            return false;

        slots[index] = null;
        Changed();
        return true;
    }

    public CollectibleItem GetItem(int index)
    {
        return IsValidSlot(index) ? slots[index] : null;
    }

    public void MoveItem(int from, int to)
    {
        if (!IsValidSlot(from) || !IsValidSlot(to) || from == to)
            return;

        (slots[from], slots[to]) = (slots[to], slots[from]);
        Changed();
    }

    public bool HasItem(CollectibleItem item)
    {
        return item != null && Array.IndexOf(slots, item) >= 0;
    }

    public bool WasCollected(CollectibleItem item)
    {
        return item != null && collected.Contains(item);
    }

    public void RefreshUI()
    {
        for (int i = 0; i < slotIcons.Length; i++)
        {
            Image icon = slotIcons[i];
            if (icon == null)
                continue;

            CollectibleItem item = i == draggingIndex ? null : GetItem(i);
            Color color = slotColors[i];
            if (item == null)
                color.a = 0f;

            icon.sprite = item != null ? item.Sprite : null;
            icon.color = color;
        }
    }
    public void BeginDrag(int index, PointerEventData eventData)
    {
        CollectibleItem item = GetItem(index);
        if (item == null || dragIcon == null)
            return;

        draggingIndex = index;
        dragIcon.sprite = item.Sprite;
        dragIcon.rectTransform.sizeDelta = slotIcons[index].rectTransform.rect.size;
        dragIcon.transform.SetAsLastSibling();
        dragIcon.gameObject.SetActive(true);
        MoveDragIcon(eventData);
        RefreshUI();
    }

    public void Drag(PointerEventData eventData)
    {
        if (draggingIndex >= 0)
            MoveDragIcon(eventData);
    }

    public void EndDrag(PointerEventData eventData)
    {
        if (draggingIndex < 0)
            return;

        int index = draggingIndex;
        CancelDrag();

        GameObject hit = eventData.pointerCurrentRaycast.gameObject;
        InventorySlot targetSlot = hit != null ? hit.GetComponentInParent<InventorySlot>() : null;
        InspectView targetView = hit != null ? hit.GetComponentInParent<InspectView>() : null;

        if (targetSlot != null && targetSlot.Inventory == this)
            MoveItem(index, targetSlot.Index);
        else if (targetView != null)
            UseOnView(index, targetView);
        else if (hit == null || !(hit.transform is RectTransform))
            UseOnWorld(index, eventData.position);
    }

    private void UseOnWorld(int index, Vector2 screenPoint)
    {
        if (PointAndClick.Current == null || InspectionManager.IsOpen)
            return;

        WorldItem target = PointAndClick.Current.GetItemAtScreenPoint(screenPoint);
        CollectibleItem item = GetItem(index);

        if (target != null && target.UseItem(item) && item.DestroyOnUse)
            RemoveItem(item);
    }

    private void UseOnView(int index, InspectView view)
    {
        CollectibleItem item = GetItem(index);

        if (view.OnItemDropped(item) && item.DestroyOnUse)
            RemoveItem(item);
    }

    private void CancelDrag()
    {
        draggingIndex = -1;
        if (dragIcon != null)
            dragIcon.gameObject.SetActive(false);
        if (slots != null)
            RefreshUI();
    }

    private void CreateDragIcon()
    {
        Image template = Array.Find(slotIcons, icon => icon != null);
        if (template == null || template.canvas == null)
            return;

        dragCanvas = (RectTransform)template.canvas.rootCanvas.transform;

        dragIcon = new GameObject("DragIcon", typeof(RectTransform), typeof(Image)).GetComponent<Image>();
        dragIcon.transform.SetParent(dragCanvas, false);
        dragIcon.raycastTarget = false;
        dragIcon.preserveAspect = true;
        dragIcon.gameObject.SetActive(false);
    }

    private void MoveDragIcon(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(dragCanvas, eventData.position, eventData.pressEventCamera, out Vector3 position))
            dragIcon.rectTransform.position = position;
    }

    private int FindEmptySlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null)
                return i;
        }
        return -1;
    }

    private bool IsValidSlot(int index)
    {
        return index >= 0 && index < slots.Length;
    }

    private void Changed()
    {
        RefreshUI();
        OnInventoryChanged?.Invoke();
    }
}