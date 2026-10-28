using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(SpriteRenderer))]
public class WorldItem : MonoBehaviour
{
    [SerializeField] private Item item;
    // Invoked when the interaction succeeds: item clicked, item collected, memory entered, or the correct item used on it
    [SerializeField] private UnityEvent onInteracted;
    // Resizes the BoxCollider to the sprite whenever the sprite is applied. Turn off to use a custom collider size
    [SerializeField] private bool fitColliderToSprite = true;

    // Items are flat images, so their collider is just a thin box over the sprite
    private const float ColliderDepth = 0.05f;

    private SpriteRenderer spriteRenderer;

    public Item Item => item;
    public UnityEvent OnInteracted => onInteracted;
    public SpriteRenderer SpriteRenderer => spriteRenderer != null ? spriteRenderer : spriteRenderer = GetComponent<SpriteRenderer>();

    private void Awake()
    {
        if (item is CollectibleItem collectible && InventoryData.WasCollected(collectible, gameObject.scene))
        {
            gameObject.SetActive(false);
            return;
        }

        ApplySprite();
    }

    private void Reset()
    {
        if (GetComponent<Collider>() == null)
            gameObject.AddComponent<BoxCollider>();
        FitColliderToSprite();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        UnityEditor.EditorApplication.delayCall += () =>
        {
            if (this != null && !UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
                ApplySprite();
        };
    }
#endif
    public void Interact()
    {
        if (item == null)
        {
            Debug.LogWarning($"[WorldItem] '{name}' has no Item assigned.");
            return;
        }

        item.OnClick(this);
    }

    public bool UseItem(CollectibleItem usedItem)
    {
        return item != null && usedItem != null && item.OnItemUsed(usedItem, this);
    }
    private void ApplySprite()
    {
        if (item != null && item.Sprite != null && SpriteRenderer.sprite != item.Sprite)
            SpriteRenderer.sprite = item.Sprite;
        FitColliderToSprite();
    }

    private void FitColliderToSprite()
    {
        Sprite sprite = SpriteRenderer.sprite;
        if (!fitColliderToSprite || sprite == null || !TryGetComponent(out BoxCollider box))
            return;

        Bounds bounds = sprite.bounds;
        Vector3 size = new Vector3(bounds.size.x, bounds.size.y, ColliderDepth);
        // Only written when it changed, so the object isn't marked as modified every time it's validated
        if (box.center != bounds.center)
            box.center = bounds.center;
        if (box.size != size)
            box.size = size;
    }
}