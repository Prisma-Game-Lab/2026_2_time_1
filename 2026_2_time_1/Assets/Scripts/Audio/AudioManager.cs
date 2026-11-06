using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    [Header("Volume")]
    [Range(0, 1)]
    public float masterVolume = 1;
    [Range(0, 1)]
    public float musicVolume = 1;
    [Range(0, 1)]
    public float ambienceVolume = 1;
    [Range(0, 1)]
    public float SFXVolume = 1;

    private Bus masterBus;
    private Bus musicBus;
    private Bus ambienceBus;
    private Bus sfxBus;

    private List<EventInstance> eventInstances;
    private List<StudioEventEmitter> eventEmitters;

    private EventInstance ambienceEventInstance;
    private EventInstance musicEventInstance;

    public static AudioManager instance { get; private set; }

    private void Awake()
    {
        // Se já existe um AudioManager (o persistente), destrói esta cópia
        if (instance != null && instance != this)
        {
            Debug.Log("AudioManager duplicado destruído (esperado ao voltar para a cena inicial).");
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        eventInstances = new List<EventInstance>();
        eventEmitters = new List<StudioEventEmitter>();

        masterBus = RuntimeManager.GetBus("bus:/");
        musicBus = RuntimeManager.GetBus("bus:/MX");
        ambienceBus = RuntimeManager.GetBus("bus:/Ambience");
        sfxBus = RuntimeManager.GetBus("bus:/SFX");
    }

    private void Start()
    {
        // Cópias destruídas no Awake não devem iniciar música
        if (instance != this) return;

        InitializeAmbience(FMODEvents.instance.ambience);
        InitializeMusic(FMODEvents.instance.music);
    }

    private void Update()
    {
        // Cópia destruída no Awake não tem buses inicializados
        if (instance != this) return;

        masterBus.setVolume(masterVolume);
        musicBus.setVolume(musicVolume);
        ambienceBus.setVolume(ambienceVolume);
        sfxBus.setVolume(SFXVolume);
    }

    private void OnDestroy()
    {
        // Só o AudioManager original faz a limpeza
        if (instance == this)
            CleanUp();
    }

    private void InitializeAmbience(EventReference ambienceEventReference)
    {
        ambienceEventInstance = CreateInstance(ambienceEventReference);
        ambienceEventInstance.start();
    }

    private void InitializeMusic(EventReference musicEventReference)
    {
        musicEventInstance = CreateInstance(musicEventReference);
        musicEventInstance.start();
    }

    // public void SetAmbienceParameter(string parameterName, float parameterValue)
    // {
    //     ambienceEventInstance.setParameterByName(parameterName, parameterValue);
    // }

    // public void SetMusicArea(MusicArea area)
    // {
    //     musicEventInstance.setParameterByName("area", (float) area);
    // }

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    public EventInstance CreateInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstances.Add(eventInstance);
        return eventInstance;
    }

    public StudioEventEmitter InitializeEventEmitter(EventReference eventReference, GameObject emitterGameObject)
    {
        StudioEventEmitter emitter = emitterGameObject.GetComponent<StudioEventEmitter>();
        emitter.EventReference = eventReference;
        eventEmitters.Add(emitter);
        return emitter;
    }

    private void CleanUp()
    {
        // stop and release any created instances
        foreach (EventInstance eventInstance in eventInstances)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }
        eventInstances.Clear();

        // stop all of the event emitters, because if we don't they may hang around in other scenes
        foreach (StudioEventEmitter emitter in eventEmitters)
        {
            if (emitter != null)
                emitter.Stop();
        }
        eventEmitters.Clear();
    }

    // parametro de nota para piano
    public void PlayPianoNote(
        EventReference pianoEvent,
        Vector3 position,
        int note)
    {
        if (pianoEvent.IsNull)
        {
            Debug.LogError("Piano EventReference is empty.");
            return;
        }

        var pianoInstance = RuntimeManager.CreateInstance(pianoEvent);

        if (!pianoInstance.isValid())
        {
            Debug.LogError(
                $"FMOD failed to create piano event: {pianoEvent.Path}"
            );
            return;
        }

        FMOD.RESULT parameterResult =
            pianoInstance.setParameterByName("Note", note);

        if (parameterResult != FMOD.RESULT.OK)
        {
            Debug.LogError($"Failed to set Note parameter: {parameterResult}");
            pianoInstance.release();
            return;
        }

        pianoInstance.set3DAttributes(RuntimeUtils.To3DAttributes(position));

        FMOD.RESULT startResult = pianoInstance.start();

        if (startResult != FMOD.RESULT.OK)
        {
            Debug.LogError($"Failed to start piano event: {startResult}");
        }

        pianoInstance.release();
    }
}