using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MemoryMapBoard : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private float zoomTime = 0.35f;
    [SerializeField] private float padding = 60f;

    private RectTransform board;
    private RectTransform viewport;
    private MemoryMapArea[] areas;
    private Vector2 homePosition;
    private Vector3 homeScale;

    private Vector2 fromPosition, toPosition;
    private Vector3 fromScale, toScale;
    private float progress = 1f;

    public MemoryMapArea CurrentArea { get; private set; }
    public bool IsZoomedIn => CurrentArea != null;

    private void Awake()
    {
        board = (RectTransform)transform;
        viewport = (RectTransform)board.parent;
        areas = GetComponentsInChildren<MemoryMapArea>(true);
        homePosition = board.anchoredPosition;
        homeScale = board.localScale;
    }

    public void ShowCurrentArea()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        foreach (MemoryMapArea area in areas)
        {
            if (area.HasScene(sceneName) && area.IsVisible)
            {
                ZoomTo(area, instant: true);
                return;
            }
        }
        ZoomOut(instant: true);
    }

    public void ZoomTo(MemoryMapArea area) => ZoomTo(area, instant: false);

    public void ZoomTo(MemoryMapArea area, bool instant)
    {
        if (area == null)
            return;

        Rect rect = area.Rect.rect;
        Vector2 center = board.InverseTransformPoint(area.Rect.TransformPoint(rect.center));
        Vector2 size = rect.size * (area.Rect.lossyScale.x / board.lossyScale.x);
        Vector2 space = viewport.rect.size - Vector2.one * (padding * 2f);
        float scale = Mathf.Min(space.x / size.x, space.y / size.y);

        CurrentArea = area;
        MoveTo(-center * scale, new Vector3(scale, scale, 1f), instant);
    }

    public void ZoomOut() => ZoomOut(instant: false);

    public void ZoomOut(bool instant)
    {
        CurrentArea = null;
        MoveTo(homePosition, homeScale, instant);
    }

    public MemoryMapArea AreaOf(MemoryMapCard card)
    {
        if (card == null)
            return null;

        foreach (MemoryMapArea area in areas)
        {
            if (area.Contains(card.transform.position))
                return area;
        }
        return null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        foreach (MemoryMapArea area in areas)
        {
            if (area != CurrentArea && area.IsVisible && RectTransformUtility.RectangleContainsScreenPoint(area.Rect, eventData.position, eventData.pressEventCamera))
            {
                ZoomTo(area);
                return;
            }
        }
    }

    private void MoveTo(Vector2 position, Vector3 scale, bool instant)
    {
        fromPosition = board.anchoredPosition;
        fromScale = board.localScale;
        toPosition = position;
        toScale = scale;
        progress = instant || zoomTime <= 0f ? 1f : 0f;
        Apply();
    }

    private void Update()
    {
        if (progress >= 1f)
            return;

        progress = Mathf.Min(1f, progress + Time.unscaledDeltaTime / zoomTime);
        Apply();
    }

    private void Apply()
    {
        float t = Mathf.SmoothStep(0f, 1f, progress);
        board.anchoredPosition = Vector2.Lerp(fromPosition, toPosition, t);
        board.localScale = Vector3.Lerp(fromScale, toScale, t);
    }
}
