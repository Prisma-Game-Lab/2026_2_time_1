using UnityEngine;

[CreateAssetMenu(fileName = "NewCollectibleItem", menuName = "Items/Collectible Item")]
public class CollectibleItem : Item
{
    [SerializeField] private bool destroyOnUse;

    
    public bool DestroyOnUse => destroyOnUse;

    public override void OnClick(WorldItem source)
    {
        if (Inventory.Current == null)
        {
            Debug.LogWarning($"[CollectibleItem] No Inventory in the scene, can't collect '{ItemName}'.");
            return;
        }

        if (Inventory.Current.AddItem(this))
        {
            source.OnInteracted.Invoke();
            source.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log($"[CollectibleItem] Inventory is full, can't collect '{ItemName}'.");
        }
    }
}
