using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewConnectionTile", menuName = "Puzzles/Connection Tile")]
public class ConnectionTile : ScriptableObject
{
    [SerializeField] private Sprite sprite;
    [SerializeField] private Sprite poweredSprite;
    [SerializeField] private ConnectionSides openings = ConnectionSides.Left | ConnectionSides.Right;

    public Sprite Sprite => sprite;
    public Sprite PoweredSprite => poweredSprite != null ? poweredSprite : sprite;
    public ConnectionSides Openings => openings;

    public ConnectionSides GetOpenings(int rotation)
    {
        return ConnectionSidesUtility.Rotate(openings, rotation);
    }
}