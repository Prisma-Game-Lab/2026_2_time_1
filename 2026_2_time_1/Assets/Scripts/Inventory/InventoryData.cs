using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class InventoryData
{
    public class Container
    {
        public readonly CollectibleItem[] Slots = new CollectibleItem[Inventory.MaxSlots];
        public readonly HashSet<CollectibleItem> Collected = new HashSet<CollectibleItem>();
    }

    public const string MainKey = "__Main__";

    private static readonly Dictionary<string, Container> containers = new Dictionary<string, Container>();
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetAll()
    {
        containers.Clear();
    }
    public static string KeyFor(Scene scene)
    {
        GameManager gm = GameManager.Instance;
        return gm != null && gm.IsMemoryScene(scene.name) ? scene.name : MainKey;
    }
    public static bool IsMemoryKey(string key)
    {
        return !string.IsNullOrEmpty(key) && key != MainKey;
    }
    public static Container Get(string key)
    {
        if (!containers.TryGetValue(key, out Container container))
        {
            container = new Container();
            containers.Add(key, container);
        }
        return container;
    }
    public static void Clear(string key)
    {
        containers.Remove(key);
    }
    public static bool WasCollected(CollectibleItem item, Scene scene)
    {
        return item != null && Get(KeyFor(scene)).Collected.Contains(item);
    }
}