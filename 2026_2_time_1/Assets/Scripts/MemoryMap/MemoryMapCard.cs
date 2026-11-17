using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class MemoryMapCard : MonoBehaviour
{
    [SerializeField] private MemoryEntry entry;
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text description;
    [SerializeField] private Image image;

    private CanvasGroup group;

    public MemoryEntry Entry => entry;

    private void Awake()
    {
        group = GetComponent<CanvasGroup>();
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

    private void Refresh()
    {
        bool discovered = MemoryMapData.IsDiscovered(entry);
        group.alpha = discovered ? 1f : 0f;
        group.blocksRaycasts = discovered;
        group.interactable = discovered;
        if (!discovered)
            return;

        if (title != null)
            title.text = entry.Title;
        if (description != null)
            description.text = MemoryMapData.GetDescription(entry);
        if (image != null && entry.Image != null)
            image.sprite = entry.Image;
    }
}
