using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "NewConnectionPuzzle", menuName = "Puzzles/Connection Puzzle Layout")]
public class ConnectionPuzzleLayout : ScriptableObject
{
    [Serializable]
    public struct CellSetup
    {
        public ConnectionTile tile;
        [Range(0, 3)] public int rotation;
        public bool fixedPosition;
        public bool fixedRotation;
    }

    [SerializeField, Min(1)] private int width = 4;
    [SerializeField, Min(1)] private int height = 3;
    [SerializeField] private CellSetup[] cells = new CellSetup[12];

    [Header("Inicio e fim")]
    [SerializeField] private Vector2Int startCell = new Vector2Int(0, 0);
    [SerializeField] private ConnectionSides startSide = ConnectionSides.Left;
    [SerializeField] private Vector2Int endCell = new Vector2Int(3, 2);
    [SerializeField] private ConnectionSides endSide = ConnectionSides.Right;

    public int Width => width;
    public int Height => height;
    public int CellCount => width * height;
    public Vector2Int StartCell => startCell;
    public ConnectionSides StartSide => startSide;
    public Vector2Int EndCell => endCell;
    public ConnectionSides EndSide => endSide;

    public CellSetup GetCell(int index)
    {
        return cells != null && index >= 0 && index < cells.Length ? cells[index] : default;
    }
    public bool Contains(Vector2Int cell)
    {
        return cell.x >= 0 && cell.x < width && cell.y >= 0 && cell.y < height;
    }
    public int ToIndex(Vector2Int cell)
    {
        return cell.y * width + cell.x;
    }
    public Vector2Int ToCell(int index)
    {
        return new Vector2Int(index % width, index / width);
    }
    private void OnValidate()
    {
        if (cells == null || cells.Length != CellCount)
            Array.Resize(ref cells, CellCount);

        if (!Contains(startCell) || !Contains(endCell))
            Debug.LogWarning($"[ConnectionPuzzleLayout] '{name}' has its start or end cell outside the {width}x{height} grid.");
        if (!ConnectionSidesUtility.IsSingle(startSide) || !ConnectionSidesUtility.IsSingle(endSide))
            Debug.LogWarning($"[ConnectionPuzzleLayout] '{name}': Start Side and End Side must be exactly one side each.");
    }
}