using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
public class Door : MonoBehaviour
{
    private const string FadeTrigger = "FadeOut";

    [SerializeField] private string sceneName;
    [SerializeField] private Animator fade;
    [SerializeField] private float fadeTime = 0.5f;

    private bool opening;

    public void Open()
    {
        if (opening || string.IsNullOrEmpty(sceneName))
            return;

        opening = true;
        StartCoroutine(OpenRoutine());
    }

    private IEnumerator OpenRoutine()
    {
        AsyncOperation load = SceneManager.LoadSceneAsync(sceneName);
        load.allowSceneActivation = false;

        Animator fadeAnimator = fade != null ? fade : FindFade();
        if (fadeAnimator != null)
        {
            fadeAnimator.SetTrigger(FadeTrigger);
            yield return new WaitForSecondsRealtime(fadeTime);
        }

        while (load.progress < 0.9f)
            yield return null;
        load.allowSceneActivation = true;
    }

    private Animator FindFade()
    {
        foreach (Animator animator in FindObjectsOfType<Animator>())
        {
            if (animator.gameObject.scene != gameObject.scene || animator.runtimeAnimatorController == null)
                continue;

            foreach (AnimatorControllerParameter parameter in animator.parameters)
            {
                if (parameter.name == FadeTrigger && parameter.type == AnimatorControllerParameterType.Trigger)
                    return animator;
            }
        }
        return null;
    }
}
