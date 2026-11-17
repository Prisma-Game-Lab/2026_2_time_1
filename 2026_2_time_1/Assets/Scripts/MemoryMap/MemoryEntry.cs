using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMemoryEntry", menuName = "Memory Map/Entry")]
public class MemoryEntry : ScriptableObject
{
    [Serializable]
    public class Line
    {
        public string key;
        [TextArea(2, 5)] public string text;
        public bool needsEntryFound;
    }

    [SerializeField] private string title;
    [SerializeField] private Sprite image;
    [SerializeField] private List<Line> lines = new List<Line>();

    public string Title => string.IsNullOrEmpty(title) ? name : title;
    public Sprite Image => image;
    public IReadOnlyList<Line> Lines => lines;

    public void Reveal(int lineIndex)
    {
        MemoryMapData.Reveal(this, lineIndex);
    }
}
