using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    private enum VolumeType
    {
        MASTER,
        MUSIC,
        AMBIENCE,
        SFX
    }

    [Header("Type")]
    [SerializeField] private VolumeType volumeType;

    private Slider volumeSlider;

    private void Awake()
    {
        volumeSlider = GetComponent<Slider>();

        if (volumeSlider == null)
        {
            Debug.LogError("VolumeSlider: No Slider component found on " + gameObject.name);
        }
    }

    private void Start()
    {
        UpdateSliderValue();
    }

    private void UpdateSliderValue()
    {
        if (volumeSlider == null)
        {
            Debug.LogError("VolumeSlider: Slider is null on " + gameObject.name);
            return;
        }

        if (AudioManager.instance == null)
        {
            Debug.LogError("VolumeSlider: AudioManager.instance is null!");
            return;
        }

        switch (volumeType)
        {
            case VolumeType.MASTER:
                volumeSlider.SetValueWithoutNotify(AudioManager.instance.masterVolume);
                break;

            case VolumeType.MUSIC:
                volumeSlider.SetValueWithoutNotify(AudioManager.instance.musicVolume);
                break;

            case VolumeType.AMBIENCE:
                volumeSlider.SetValueWithoutNotify(AudioManager.instance.ambienceVolume);
                break;

            case VolumeType.SFX:
                volumeSlider.SetValueWithoutNotify(AudioManager.instance.SFXVolume);
                break;
        }
    }

    public void OnSliderValueChanged()
    {
        if (volumeSlider == null || AudioManager.instance == null)
        {
            Debug.LogError("VolumeSlider: Missing Slider or AudioManager reference!");
            return;
        }

        switch (volumeType)
        {
            case VolumeType.MASTER:
                AudioManager.instance.masterVolume = volumeSlider.value;
                break;

            case VolumeType.MUSIC:
                AudioManager.instance.musicVolume = volumeSlider.value;
                break;

            case VolumeType.AMBIENCE:
                AudioManager.instance.ambienceVolume = volumeSlider.value;
                break;

            case VolumeType.SFX:
                AudioManager.instance.SFXVolume = volumeSlider.value;
                break;
        }
    }
}