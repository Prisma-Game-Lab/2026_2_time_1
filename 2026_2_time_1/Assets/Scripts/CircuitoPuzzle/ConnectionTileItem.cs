using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewConnectionTileItem", menuName = "Items/Connection Tile Item")]
public class ConnectionTileItem : CollectibleItem
{
    [SerializeField] private ConnectionTile tile;
    [SerializeField, Range(0, 3)] private int rotation;

    public ConnectionTile Tile => tile;
    public int Rotation => rotation;
}