
using UnityEngine;
using FMODUnity;

[CreateAssetMenu(fileName = "NewClickableItem", menuName = "Items/Clickable Item")]
public class ClickableItem : Item
{
    [SerializeField] private EventReference clickedObjectSound;

    public override void OnClick(WorldItem source)
    {
        source.OnInteracted.Invoke();

        AudioManager.instance.PlayOneShot(
            clickedObjectSound,
            source.transform.position
        );
    }
}
