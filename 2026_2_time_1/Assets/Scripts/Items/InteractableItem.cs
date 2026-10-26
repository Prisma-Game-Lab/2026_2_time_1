using UnityEngine;

[CreateAssetMenu(fileName = "NewInteractableItem", menuName = "Items/Interactable Item")]
public class InteractableItem : Item
{
    [SerializeField] private CollectibleItem requiredItem;

    public CollectibleItem RequiredItem => requiredItem;

    
    public override void OnClick(WorldItem source)
    {
    }

    public override bool OnItemUsed(CollectibleItem usedItem, WorldItem source)
    {
        if (requiredItem == null || usedItem != requiredItem)
        {
            Debug.Log($"[InteractableItem] '{usedItem.ItemName}' doesn't work on '{ItemName}'.");
            return false;
        }

        source.OnInteracted.Invoke();
        return true;
    }
}
