using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
[DisallowMultipleComponent]
public class ConnectionPuzzleState : MonoBehaviour
{
    [Serializable]
    public struct Slot
    {
        public ConnectionTile tile;
        public int rotation;
        public bool fixedRotation;
    }
    private class Data
    {
        public ConnectionPuzzleLayout Layout;
        public Slot[] Slots;
        public bool IsSolved;
    }

    private static readonly Dictionary<string, Data> saved = new Dictionary<string, Data>();

    private Data data;

    public ConnectionPuzzleLayout Layout => data?.Layout;
    public Slot[] Slots => data?.Slots;
    public bool IsSolved
    {
        get => data != null && data.IsSolved;
        set { if (data != null) data.IsSolved = value; }
    }

    public static ConnectionPuzzleState GetOrCreate(GameObject owner, ConnectionPuzzleLayout layout)
    {
        return GetOrCreate(owner, layout, null);
    }
    public static ConnectionPuzzleState GetOrCreate(GameObject owner, ConnectionPuzzleLayout layout, string saveKey)
    {
        if (!owner.TryGetComponent(out ConnectionPuzzleState state))
            state = owner.AddComponent<ConnectionPuzzleState>();
        if (state.data == null)
        {
            if (string.IsNullOrEmpty(saveKey) || !saved.TryGetValue(saveKey, out state.data))
            {
                state.data = new Data();
                if (!string.IsNullOrEmpty(saveKey))
                    saved[saveKey] = state.data;
            }
        }
        if (state.Layout != layout || state.Slots == null || state.Slots.Length != layout.CellCount)
            state.ResetTo(layout);
        return state;
    }
    public void ResetTo(ConnectionPuzzleLayout layout)
    {
        data.Layout = layout;
        data.IsSolved = false;
        data.Slots = new Slot[layout.CellCount];
        for (int i = 0; i < data.Slots.Length; i++)
        {
            ConnectionPuzzleLayout.CellSetup setup = layout.GetCell(i);
            data.Slots[i] = new Slot
            {
                tile = setup.tile,
                rotation = setup.rotation,
                fixedRotation = setup.fixedRotation,
            };
        }
    }
    public static void ClearAll() => saved.Clear();
}