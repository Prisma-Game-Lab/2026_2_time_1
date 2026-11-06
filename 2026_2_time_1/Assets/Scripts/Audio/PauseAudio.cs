
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class PauseAudioController : MonoBehaviour
{
    [Header("FMOD")]
    [SerializeField] private string eventPath = "event:/GameStatus";

    [Header("Parameter")]
    [SerializeField] private string parameterName = "GamePaused";

    // Set these according to your FMOD parameter values.
    [SerializeField] private float pausedValue = 0f;
    [SerializeField] private float runningValue = 1f;

    private EventInstance gameStatusInstance;
    private bool isPaused;

    private void Start()
    {
        gameStatusInstance = RuntimeManager.CreateInstance(eventPath);

        // Start the event once.
        gameStatusInstance.start();

        SetPaused(false);
    }

    public void SetPaused(bool pause)
    {
        if (isPaused == pause)
            return;

        isPaused = pause;

        // Update the global FMOD parameter.
        RuntimeManager.StudioSystem.setParameterByName(
            parameterName,
            pause ? pausedValue : runningValue
        );

        // Pause or resume Unity gameplay.
        Time.timeScale = pause ? 0f : 1f;
    }

    public void TogglePause()
    {
        SetPaused(!isPaused);
    }

    private void OnDestroy()
    {
        gameStatusInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        gameStatusInstance.release();

        Time.timeScale = 1f;
    }
}
