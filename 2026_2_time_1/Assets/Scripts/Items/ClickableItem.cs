using UnityEngine;

[CreateAssetMenu(fileName = "NewClickableItem", menuName = "Items/Clickable Item")]
public class ClickableItem : Item
{
    
    public override void OnClick(WorldItem source)
    {
        source.OnInteracted.Invoke();
    }
}
