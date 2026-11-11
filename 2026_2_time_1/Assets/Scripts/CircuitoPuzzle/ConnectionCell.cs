using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ConnectionCell : MonoBehaviour, IPointerDownHandler, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image tileImage;

    private bool dragStarted;
    private bool draggingTile;
    private float currentAngle;
    private float targetAngle;

    public ConnectionPuzzleView View { get; private set; }
    public int Index { get; private set; }
    public Image TileImage => tileImage;
    public RectTransform Rect => (RectTransform)transform;

    public void Init(ConnectionPuzzleView view, int index)
    {
        View = view;
        Index = index;
        if (!TryGetComponent(out Graphic background))
        {
            background = gameObject.AddComponent<Image>();
            background.color = Color.clear;
        }
        background.raycastTarget = true;

        if (tileImage == null)
        {
            tileImage = new GameObject("Tile", typeof(RectTransform), typeof(Image)).GetComponent<Image>();
            RectTransform rect = tileImage.rectTransform;
            rect.SetParent(transform, false);
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
        tileImage.raycastTarget = false;
        tileImage.preserveAspect = true;
    }
    public void SetTile(Sprite sprite, Color color, int rotation, bool instant)
    {
        tileImage.sprite = sprite;
        tileImage.color = color;
        tileImage.enabled = sprite != null;

        targetAngle = -90f * rotation;
        if (instant)
            currentAngle = targetAngle;
        ApplyAngle();
    }
    public void SetTileVisible(bool visible)
    {
        tileImage.enabled = visible && tileImage.sprite != null;
    }

    private void Update()
    {
        if (View == null || Mathf.Approximately(Mathf.DeltaAngle(currentAngle, targetAngle), 0f))
            return;
        currentAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, View.RotationSpeed * Time.unscaledDeltaTime);
        ApplyAngle();
    }

    private void ApplyAngle()
    {
        tileImage.rectTransform.localEulerAngles = new Vector3(0f, 0f, currentAngle);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        dragStarted = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (View == null || dragStarted)
            return;

        if (eventData.button == PointerEventData.InputButton.Left)
            View.RotateCell(Index, 1);
        else if (eventData.button == PointerEventData.InputButton.Right)
            View.RotateCell(Index, -1);
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        dragStarted = true;
        draggingTile = View != null && eventData.button == PointerEventData.InputButton.Left && View.BeginDrag(Index, eventData);
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (draggingTile)
            View.Drag(eventData);
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!draggingTile)
            return;

        draggingTile = false;
        View.EndDrag(Index, eventData);
    }
}