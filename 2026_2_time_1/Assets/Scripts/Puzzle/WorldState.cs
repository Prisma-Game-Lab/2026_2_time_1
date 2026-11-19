using System.Collections;
using System.Collections.Generic;
public static class WorldState
{
    private static readonly HashSet<string> flags = new HashSet<string>();
    public static bool Has(string id) => !string.IsNullOrEmpty(id) && flags.Contains(id);
    public static void Set(string id)
    {
        if (!string.IsNullOrEmpty(id) && flags.Add(id))
            UnityEngine.Debug.Log($"[WorldState] Set '{id}'");
    }
    public static void Clear() => flags.Clear();
}