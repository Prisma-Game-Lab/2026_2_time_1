using UnityEngine;

public abstract class Item : ScriptableObject
{
    [SerializeField] private string itemName;
    [SerializeField] private Sprite sprite;

    public string ItemName => string.IsNullOrEmpty(itemName) ? name : itemName;
    public Sprite Sprite => sprite;

    public abstract void OnClick(WorldItem source);

    public virtual bool OnItemUsed(CollectibleItem usedItem, WorldItem source)
    {
        return false;
    }
}
