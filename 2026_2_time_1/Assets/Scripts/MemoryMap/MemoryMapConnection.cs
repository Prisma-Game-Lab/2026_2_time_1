using UnityEngine;
using UnityEngine.EventSystems;

[ExecuteAlways]
[RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
public class MemoryMapConnection : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private MemoryMapCard from;
    [SerializeField] private MemoryMapCard to;
    [SerializeField] private MemoryEntry requiredEntry;
    [SerializeField] private int requiredLine;

    private void OnEnable()
    {
        if (!Application.isPlaying)
            return;

        MemoryMapData.Changed += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        MemoryMapData.Changed -= Refresh;
    }

    // Clicking the string zooms to the area of the card at its other end
    public void OnPointerClick(PointerEventData eventData)
    {
        MemoryMapBoard board = GetComponentInParent<MemoryMapBoard>();
        if (eventData.button != PointerEventData.InputButton.Left || board == null)
            return;

        MemoryMapArea fromArea = board.AreaOf(from);
        MemoryMapArea toArea = board.AreaOf(to);
        if (fromArea != toArea)
            board.ZoomTo(board.CurrentArea == toArea ? fromArea : toArea);
    }

    private void LateUpdate()
    {
        if (from != null && to != null)
            Stretch();
    }

    private void Refresh()
    {
        bool visible = from != null && to != null
            && MemoryMapData.IsDiscovered(from.Entry) && MemoryMapData.IsDiscovered(to.Entry)
            && (requiredEntry == null || MemoryMapData.IsRevealed(requiredEntry, requiredLine));

        CanvasGroup group = GetComponent<CanvasGroup>();
        group.alpha = visible ? 1f : 0f;
        group.blocksRaycasts = visible;
    }

    
    private void Stretch()
    {
        RectTransform rect = (RectTransform)transform;
        Transform parent = rect.parent;
        Vector2 a = parent != null ? (Vector2)parent.InverseTransformPoint(from.transform.position) : (Vector2)from.transform.position;
        Vector2 b = parent != null ? (Vector2)parent.InverseTransformPoint(to.transform.position) : (Vector2)to.transform.position;
        Vector2 delta = b - a;

        Vector3 position = (a + b) * 0.5f;
        Vector2 size = new Vector2(delta.magnitude, rect.sizeDelta.y);
        Quaternion rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg);

        // Width is the string's length, which only works with non-stretched anchors
        if (rect.anchorMin != rect.anchorMax)
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        if (rect.pivot != new Vector2(0.5f, 0.5f))
            rect.pivot = new Vector2(0.5f, 0.5f);
        if ((rect.localPosition - position).sqrMagnitude > 0.0001f)
            rect.localPosition = position;
        if ((rect.sizeDelta - size).sqrMagnitude > 0.0001f)
            rect.sizeDelta = size;
        if (Quaternion.Angle(rect.localRotation, rotation) > 0.01f)
            rect.localRotation = rotation;
    }
}
