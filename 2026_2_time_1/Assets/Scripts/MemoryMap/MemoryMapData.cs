using System;
using System.Collections.Generic;
using UnityEngine;


public static class MemoryMapData
{
    private static readonly Dictionary<MemoryEntry, List<int>> shown = new Dictionary<MemoryEntry, List<int>>();
    
    private static readonly HashSet<MemoryEntry> discovered = new HashSet<MemoryEntry>();
    private static readonly HashSet<MemoryEntry.Line> revealed = new HashSet<MemoryEntry.Line>();

    public static event Action Changed;

    // Index of the current objective in MemoryObjectives
    public static int ObjectiveIndex { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetData()
    {
        shown.Clear();
        discovered.Clear();
        revealed.Clear();
        ObjectiveIndex = 0;
        Changed = null;
    }

    public static void AdvanceObjective()
    {
        ObjectiveIndex++;
        Changed?.Invoke();
    }

    public static void Reveal(MemoryEntry entry, int lineIndex)
    {
        if (entry == null || lineIndex < 0 || lineIndex >= entry.Lines.Count)
        {
            Debug.LogWarning($"[MemoryMapData] '{(entry != null ? entry.name : "null")}' has no line {lineIndex}.", entry);
            return;
        }

        MemoryEntry.Line line = entry.Lines[lineIndex];
        if (!revealed.Add(line))
            return;

        if (!shown.TryGetValue(entry, out List<int> lines))
            shown[entry] = lines = new List<int>();

        int sameKey = string.IsNullOrEmpty(line.key) ? -1 : lines.FindIndex(i => entry.Lines[i].key == line.key);
        if (sameKey < 0)
            lines.Add(lineIndex);
        else if (lines[sameKey] < lineIndex)
            lines[sameKey] = lineIndex;

        if (!line.needsEntryFound)
            discovered.Add(entry);

        Changed?.Invoke();
    }

    public static bool IsRevealed(MemoryEntry entry, int lineIndex)
    {
        return entry != null && lineIndex >= 0 && lineIndex < entry.Lines.Count && revealed.Contains(entry.Lines[lineIndex]);
    }

    public static bool IsDiscovered(MemoryEntry entry)
    {
        return entry != null && discovered.Contains(entry);
    }

    public static string GetDescription(MemoryEntry entry)
    {
        return IsDiscovered(entry) && shown.TryGetValue(entry, out List<int> lines)
            ? string.Join("\n", lines.ConvertAll(i => entry.Lines[i].text))
            : "";
    }
}
