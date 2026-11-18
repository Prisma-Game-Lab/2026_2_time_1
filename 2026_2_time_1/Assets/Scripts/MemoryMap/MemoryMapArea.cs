using System;
using UnityEngine;

// A region of the Memory Map board. Cards belong to the area they sit on top of (no need to parent them).
// Hidden until one of its cards is discovered.
[RequireComponent(typeof(CanvasGroup))]
public class MemoryMapArea : MonoBehaviour
{
    [SerializeField] private string[] scenes = new string[0];

    private CanvasGroup group;
    private MemoryMapCard[] cards = new MemoryMapCard[0];

    public RectTransform Rect => (RectTransform)transform;

    // Any discovered card on top of this area
    public bool IsVisible
    {
        get
        {
            foreach (MemoryMapCard card in cards)
            {
                if (MemoryMapData.IsDiscovered(card.Entry) && Contains(card.transform.position))
                    return true;
            }
            return false;
        }
    }

    private void Awake()
    {
        group = GetComponent<CanvasGroup>();
        MemoryMapBoard board = GetComponentInParent<MemoryMapBoard>(true);
        if (board != null)
            cards = board.GetComponentsInChildren<MemoryMapCard>(true);
        else
            Debug.LogWarning($"[MemoryMapArea] '{name}' isn't inside a MemoryMapBoard.", this);
    }

    private void OnEnable()
    {
        MemoryMapData.Changed += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        MemoryMapData.Changed -= Refresh;
    }

    public bool HasScene(string sceneName) => Array.IndexOf(scenes, sceneName) >= 0;

    public bool Contains(Vector3 worldPoint) => Rect.rect.Contains(Rect.InverseTransformPoint(worldPoint));

    private void Refresh()
    {
        bool visible = IsVisible;
        group.alpha = visible ? 1f : 0f;
        group.blocksRaycasts = visible;
    }
}
