using System;
using UnityEngine;

[RequireComponent(typeof(PointAndClick))]
public class GameCursor : MonoBehaviour
{
    [Serializable]
    public struct CursorImage
    {
        public Texture2D texture;
        public Vector2 hotspot;
    }

    [SerializeField] private CursorImage normal;
    [SerializeField] private CursorImage hover;
    [SerializeField] private CursorImage holding;
    [SerializeField] private CursorImage door;

    public static bool Holding { get; set; }

    private PointAndClick pointAndClick;
    private Texture2D current;
    private bool applied;

    private void Awake()
    {
        pointAndClick = GetComponent<PointAndClick>();
    }

    private void OnDisable()
    {
        Holding = false;
        applied = false;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    private void Update()
    {
        CursorImage image = Pick();
        if (applied && image.texture == current)
            return;

        current = image.texture;
        applied = true;
        Cursor.SetCursor(image.texture, image.hotspot, CursorMode.Auto);
    }

    private CursorImage Pick()
    {
        if (Holding)
            return holding;
        if (pointAndClick.IsBlocked)
            return normal;

        Vector2 pointer = pointAndClick.PointerPosition;
        if (pointAndClick.GetItemAtScreenPoint(pointer) != null)
            return hover;
        if (pointAndClick.GetDoorAtScreenPoint(pointer) != null)
            return door;
        return normal;
    }
}
