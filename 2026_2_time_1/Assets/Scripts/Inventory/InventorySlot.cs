using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Inventory Inventory { get; private set; }
    public int Index { get; private set; }

    public void Init(Inventory inventory, int index)
    {
        Inventory = inventory;
        Index = index;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (Inventory != null)
            Inventory.BeginDrag(Index, eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (Inventory != null)
            Inventory.Drag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (Inventory != null)
            Inventory.EndDrag(eventData);
    }
}
