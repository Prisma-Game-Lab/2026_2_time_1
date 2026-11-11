using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ConnectionPuzzleView : InspectView
{
    [SerializeField] private ConnectionPuzzleLayout layout;
    [SerializeField] private RectTransform board;
    [SerializeField] private ConnectionCell cellPrefab;
    [SerializeField] private float cellPadding = 4f;

    [Header("Look")]
    [SerializeField] private Sprite slotSprite;
    [SerializeField] private Color slotColor = new Color(1f, 1f, 1f, 0.08f);
    [SerializeField] private Color fixedSlotColor = new Color(1f, 1f, 1f, 0.02f);
    [SerializeField] private Color idleColor = new Color(0.55f, 0.55f, 0.55f);
    [SerializeField] private Color poweredColor = new Color(1f, 0.85f, 0.45f);
    [SerializeField] private float rotationSpeed = 720f;

    [SerializeField] private float completeDelay = 0.8f;

    [Header("Events (Fmod aqui)")]
    [SerializeField] private UnityEvent onTileRotated = new UnityEvent();
    [SerializeField] private UnityEvent onTilesSwapped = new UnityEvent();
    [SerializeField] private UnityEvent onTilePlaced = new UnityEvent();
    [SerializeField] private UnityEvent onSolved = new UnityEvent();

    private ConnectionPuzzleState state;
    private ConnectionCell[] cells;
    private bool[] powered;
    private readonly Queue<int> floodQueue = new Queue<int>();

    private Image dragGhost;
    private int draggingIndex = -1;

    private bool waitingToComplete;
    private float completeTimer;

    public float RotationSpeed => rotationSpeed;
    public bool IsInteractable => state != null && !state.IsSolved && !IsCompleted;

    private void Reset()
    {
        completeOn = CompleteMode.WhenSolved;
    }

    protected override void OnOpened()
    {
        if (layout == null)
        {
            Debug.LogWarning($"[ConnectionPuzzleView] '{name}' has no Layout assigned.");
            return;
        }

        if (completeOn != CompleteMode.WhenSolved)
            Debug.LogWarning($"[ConnectionPuzzleView] '{name}' completes {completeOn}, so it won't wait for the puzzle. Set Complete On to When Solved.");

        if (board == null)
            board = (RectTransform)transform;

        GameObject owner = Source != null ? Source.gameObject : gameObject;
        state = ConnectionPuzzleState.GetOrCreate(owner, layout);

        BuildBoard();
        CreateDragGhost();
        Refresh(instant: true);
    }

    protected override void OnClosed()
    {
        CancelDrag();

        if (waitingToComplete)
        {
            waitingToComplete = false;
            Complete();
        }
    }

    private void Update()
    {
        if (!waitingToComplete)
            return;

        completeTimer -= Time.unscaledDeltaTime;
        if (completeTimer <= 0f)
        {
            waitingToComplete = false;
            Complete();
        }
    }
    public void ResetBoard()
    {
        if (!IsInteractable)
            return;

        CancelDrag();
        state.ResetTo(layout);
        Changed(instant: true);
    }
    public void RotateCell(int index, int steps)
    {
        if (!IsInteractable || draggingIndex >= 0)
            return;

        ConnectionPuzzleState.Slot slot = state.Slots[index];
        if (slot.tile == null || slot.fixedRotation)
            return;

        state.Slots[index].rotation = ((slot.rotation + steps) % 4 + 4) % 4;
        onTileRotated.Invoke();
        Changed(instant: false);
    }
    public bool BeginDrag(int index, PointerEventData eventData)
    {
        if (!IsInteractable || !CanMove(index) || state.Slots[index].tile == null || dragGhost == null)
            return false;

        draggingIndex = index;
        ConnectionCell cell = cells[index];

        dragGhost.sprite = cell.TileImage.sprite;
        dragGhost.color = cell.TileImage.color;
        dragGhost.rectTransform.sizeDelta = cell.Rect.rect.size;
        dragGhost.rectTransform.localEulerAngles = new Vector3(0f, 0f, -90f * state.Slots[index].rotation);
        dragGhost.transform.SetAsLastSibling();
        dragGhost.gameObject.SetActive(true);
        cell.SetTileVisible(false);

        Drag(eventData);
        return true;
    }
    public void Drag(PointerEventData eventData)
    {
        if (draggingIndex < 0)
            return;

        if (RectTransformUtility.ScreenPointToWorldPointInRectangle((RectTransform)transform, eventData.position, eventData.pressEventCamera, out Vector3 position))
            dragGhost.rectTransform.position = position;
    }
    public void EndDrag(int index, PointerEventData eventData)
    {
        if (draggingIndex != index)
            return;

        CancelDrag();

        GameObject hit = eventData.pointerCurrentRaycast.gameObject;
        ConnectionCell target = hit != null ? hit.GetComponentInParent<ConnectionCell>() : null;
        if (target == null || target.View != this || target.Index == index || !CanMove(target.Index))
            return;

        (state.Slots[index], state.Slots[target.Index]) = (state.Slots[target.Index], state.Slots[index]);
        onTilesSwapped.Invoke();
        Changed(instant: true);
    }
    public override bool OnItemDropped(CollectibleItem item)
    {
        if (!IsInteractable || !(item is ConnectionTileItem tileItem) || tileItem.Tile == null)
            return false;

        int index = FindFreeCellUnderPointer();
        if (index < 0)
            index = FindFreeCell();
        if (index < 0)
            return false;

        state.Slots[index] = new ConnectionPuzzleState.Slot { tile = tileItem.Tile, rotation = tileItem.Rotation };

        if (Inventory.Current != null)
            Inventory.Current.RemoveItem(item);

        onTilePlaced.Invoke();
        Changed(instant: true);
        return true;
    }

    private void Changed(bool instant)
    {
        bool solved = Refresh(instant);
        if (solved && !state.IsSolved)
            Solve();
    }

    private void Solve()
    {
        state.IsSolved = true;
        CancelDrag();
        onSolved.Invoke();

        waitingToComplete = true;
        completeTimer = completeDelay;
    }
    private bool Refresh(bool instant)
    {
        bool solved = FloodFromStart();

        for (int i = 0; i < cells.Length; i++)
        {
            ConnectionPuzzleState.Slot slot = state.Slots[i];
            Sprite sprite = slot.tile == null ? null : powered[i] ? slot.tile.PoweredSprite : slot.tile.Sprite;
            cells[i].SetTile(sprite, powered[i] ? poweredColor : idleColor, slot.rotation, instant);
        }

        if (draggingIndex >= 0)
            cells[draggingIndex].SetTileVisible(false);

        return solved;
    }
    private bool FloodFromStart()
    {
        System.Array.Clear(powered, 0, powered.Length);
        floodQueue.Clear();

        Vector2Int start = layout.StartCell;
        if (!layout.Contains(start))
            return false;

        int startIndex = layout.ToIndex(start);
        if (!IsOpen(startIndex, layout.StartSide))
            return false;

        powered[startIndex] = true;
        floodQueue.Enqueue(startIndex);

        while (floodQueue.Count > 0)
        {
            int index = floodQueue.Dequeue();
            Vector2Int cell = layout.ToCell(index);

            foreach (ConnectionSides side in ConnectionSidesUtility.All)
            {
                if (!IsOpen(index, side))
                    continue;

                Vector2Int next = cell + ConnectionSidesUtility.Offset(side);
                if (!layout.Contains(next))
                    continue;

                int nextIndex = layout.ToIndex(next);
                if (powered[nextIndex] || !IsOpen(nextIndex, ConnectionSidesUtility.Opposite(side)))
                    continue;

                powered[nextIndex] = true;
                floodQueue.Enqueue(nextIndex);
            }
        }

        Vector2Int end = layout.EndCell;
        if (!layout.Contains(end))
            return false;

        int endIndex = layout.ToIndex(end);
        return powered[endIndex] && IsOpen(endIndex, layout.EndSide);
    }

    private bool IsOpen(int index, ConnectionSides side)
    {
        ConnectionPuzzleState.Slot slot = state.Slots[index];
        return slot.tile != null && (slot.tile.GetOpenings(slot.rotation) & side) != 0;
    }

    private bool CanMove(int index)
    {
        return !layout.GetCell(index).fixedPosition;
    }

    private int FindFreeCell()
    {
        for (int i = 0; i < cells.Length; i++)
        {
            if (state.Slots[i].tile == null && CanMove(i))
                return i;
        }
        return -1;
    }
    private int FindFreeCellUnderPointer()
    {
        if (Pointer.current == null)
            return -1;

        Vector2 screenPoint = Pointer.current.position.ReadValue();
        Canvas canvas = GetComponentInParent<Canvas>();
        Camera cam = canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay ? canvas.worldCamera : null;

        for (int i = 0; i < cells.Length; i++)
        {
            if (state.Slots[i].tile == null && CanMove(i) && RectTransformUtility.RectangleContainsScreenPoint(cells[i].Rect, screenPoint, cam))
                return i;
        }
        return -1;
    }

    private void BuildBoard()
    {
        if (!board.TryGetComponent(out Graphic boardGraphic))
        {
            boardGraphic = board.gameObject.AddComponent<Image>();
            boardGraphic.color = Color.clear;
        }
        boardGraphic.raycastTarget = true;

        int width = layout.Width;
        int height = layout.Height;
        cells = new ConnectionCell[layout.CellCount];
        powered = new bool[layout.CellCount];

        for (int i = 0; i < cells.Length; i++)
        {
            Vector2Int cell = layout.ToCell(i);
            ConnectionCell slot = CreateCell(i);
            RectTransform rect = slot.Rect;
            rect.anchorMin = new Vector2((float)cell.x / width, 1f - (float)(cell.y + 1) / height);
            rect.anchorMax = new Vector2((float)(cell.x + 1) / width, 1f - (float)cell.y / height);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.offsetMin = new Vector2(cellPadding, cellPadding);
            rect.offsetMax = new Vector2(-cellPadding, -cellPadding);

            slot.Init(this, i);
            cells[i] = slot;
        }
    }

    private ConnectionCell CreateCell(int index)
    {
        if (cellPrefab != null)
        {
            ConnectionCell instance = Instantiate(cellPrefab, board, false);
            instance.name = $"Cell {index}";
            return instance;
        }

        GameObject cellObject = new GameObject($"Cell {index}", typeof(RectTransform), typeof(Image), typeof(ConnectionCell));
        cellObject.transform.SetParent(board, false);

        Image background = cellObject.GetComponent<Image>();
        background.sprite = slotSprite;
        background.color = CanMove(index) ? slotColor : fixedSlotColor;

        return cellObject.GetComponent<ConnectionCell>();
    }

    private void CreateDragGhost()
    {
        dragGhost = new GameObject("DragGhost", typeof(RectTransform), typeof(Image)).GetComponent<Image>();
        dragGhost.transform.SetParent(transform, false);
        dragGhost.raycastTarget = false;
        dragGhost.preserveAspect = true;
        dragGhost.gameObject.SetActive(false);
    }

    private void CancelDrag()
    {
        if (draggingIndex >= 0 && cells != null)
            cells[draggingIndex].SetTileVisible(true);

        draggingIndex = -1;
        if (dragGhost != null)
            dragGhost.gameObject.SetActive(false);
    }
}