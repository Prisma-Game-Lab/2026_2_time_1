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

    public ConnectionPuzzleLayout Layout { get; private set; }
    public Slot[] Slots { get; private set; }
    public bool IsSolved { get; set; }

    public static ConnectionPuzzleState GetOrCreate(GameObject owner, ConnectionPuzzleLayout layout)
    {
        if (!owner.TryGetComponent(out ConnectionPuzzleState state))
            state = owner.AddComponent<ConnectionPuzzleState>();

        if (state.Layout != layout || state.Slots == null || state.Slots.Length != layout.CellCount)
            state.ResetTo(layout);
        return state;
    }
    public void ResetTo(ConnectionPuzzleLayout layout)
    {
        Layout = layout;
        IsSolved = false;
        Slots = new Slot[layout.CellCount];
        for (int i = 0; i < Slots.Length; i++)
        {
            ConnectionPuzzleLayout.CellSetup setup = layout.GetCell(i);
            Slots[i] = new Slot
            {
                tile = setup.tile,
                rotation = setup.rotation,
                fixedRotation = setup.fixedRotation,
            };
        }
    }
}