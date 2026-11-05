using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;

public static class AudioManager
{
    public static void PlayOneShot(EventReference e, Vector3 position = default)
        => RuntimeManager.PlayOneShot(e, position);

    public static EventInstance Start(EventReference e)
    {
        EventInstance instance = RuntimeManager.CreateInstance(e);
        instance.start();
        return instance;
    }

    public static void Stop(EventInstance instance, bool fade = true)
    {
        instance.stop(fade ? FMOD.Studio.STOP_MODE.ALLOWFADEOUT : FMOD.Studio.STOP_MODE.IMMEDIATE);
        instance.release();
    }
    public static void SetGlobalParameter(string name, float value)
        => RuntimeManager.StudioSystem.setParameterByName(name, value);
}