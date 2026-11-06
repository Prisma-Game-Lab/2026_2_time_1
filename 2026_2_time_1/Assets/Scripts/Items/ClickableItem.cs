using UnityEngine;
using FMODUnity;

[CreateAssetMenu(fileName = "NewClickableItem", menuName = "Items/Clickable Item")]
public class ClickableItem : Item
{
    public enum SoundType
    {
        OneShot,
        Piano
    }

    [SerializeField] private SoundType soundType;
    [SerializeField] private EventReference clickedObjectSound;

    public override void OnClick(WorldItem source)
    {
        source.OnInteracted.Invoke();

        if (soundType == SoundType.Piano)
        {
            if (source.TryGetComponent<PianoKey>(out var pianoKey))
            {
                AudioManager.instance.PlayPianoNote(
                    clickedObjectSound,
                    source.transform.position,
                    pianoKey.Note
                );
            }
            else
            {
                Debug.LogWarning(
                    $"PianoKey component missing on {source.name}"
                );
            }
        }
        else
        {
            AudioManager.instance.PlayOneShot(
                clickedObjectSound,
                source.transform.position
            );
        }
    }
}