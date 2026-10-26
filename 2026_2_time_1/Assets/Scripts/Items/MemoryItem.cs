using UnityEngine;

[CreateAssetMenu(fileName = "NewMemoryItem", menuName = "Items/Memory Item")]
public class MemoryItem : Item
{
    [SerializeField] private string memorySceneName;

    public string MemorySceneName => memorySceneName;

    public override void OnClick(WorldItem source)
    {
        if (MemoryManager.Current == null)
        {
            Debug.LogWarning($"[MemoryItem] No MemoryManager in the scene, can't trigger memory '{memorySceneName}'.");
            return;
        }

        source.OnInteracted.Invoke();
        MemoryManager.Current.EnterMemory(memorySceneName);
    }
}
