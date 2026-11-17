using UnityEngine;

public class MemoryMapReveal : MonoBehaviour
{
    public enum When
    {
        OnClick,
        OnSuccess,
        Manual,
    }

    [SerializeField] private When when = When.OnClick;
    [SerializeField] private MemoryEntry entry;
    [SerializeField] private int[] lines = { 0 };

    private WorldItem worldItem;

    private void Awake()
    {
        worldItem = GetComponent<WorldItem>();
        if (when != When.Manual && worldItem == null)
            Debug.LogWarning($"[MemoryMapReveal] '{name}' is set to {when} but has no WorldItem.", this);
    }

    private void OnEnable()
    {
        if (worldItem == null)
            return;

        if (when == When.OnClick)
            worldItem.Clicked += Reveal;
        else if (when == When.OnSuccess)
            worldItem.OnInteracted.AddListener(Reveal);
    }

    private void OnDisable()
    {
        if (worldItem == null)
            return;

        worldItem.Clicked -= Reveal;
        worldItem.OnInteracted.RemoveListener(Reveal);
    }

    public void Reveal()
    {
        foreach (int line in lines)
            MemoryMapData.Reveal(entry, line);
    }
}
