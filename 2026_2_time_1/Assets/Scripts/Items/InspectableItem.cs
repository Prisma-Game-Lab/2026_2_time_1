using UnityEngine;

[CreateAssetMenu(fileName = "NewInspectableItem", menuName = "Items/Inspectable Item")]
public class InspectableItem : Item
{
    
    [SerializeField] private InspectView inspectView;

    public InspectView InspectView => inspectView;

    public override void OnClick(WorldItem source)
    {
        if (InspectionManager.Current == null)
        {
            Debug.LogWarning($"[InspectableItem] No InspectionManager in the scene, can't inspect '{ItemName}'.");
            return;
        }

        if (inspectView == null)
        {
            Debug.LogWarning($"[InspectableItem] '{ItemName}' has no Inspect View assigned.");
            return;
        }

        InspectionManager.Current.Open(inspectView, source);
    }
}
