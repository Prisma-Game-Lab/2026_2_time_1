using System.Collections;
using TMPro;
using UnityEngine;

// Shows the current objective. When it advances, the old one fades out and the next one fades in
[RequireComponent(typeof(TMP_Text))]
public class MemoryMapObjectiveText : MonoBehaviour
{
    [SerializeField] private MemoryObjectives objectives;
    [SerializeField] private float fadeTime = 0.5f;

    private static int shownIndex;

    private TMP_Text text;
    private Coroutine fade;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetShown() => shownIndex = 0;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        MemoryMapData.Changed += Refresh;
        text.text = TextAt(shownIndex);
        text.alpha = 1f;
        Refresh();
    }

    private void OnDisable()
    {
        MemoryMapData.Changed -= Refresh;
        fade = null;
    }

    private void Refresh()
    {
        int current = MemoryMapData.ObjectiveIndex;
        if (current == shownIndex)
            return;

        shownIndex = current;
        if (fade != null)
            StopCoroutine(fade);
        fade = StartCoroutine(FadeTo(TextAt(current)));
    }

    private IEnumerator FadeTo(string next)
    {
        if (!string.IsNullOrEmpty(text.text))
            yield return Fade(1f, 0f);

        text.text = next;
        yield return Fade(0f, 1f);
        fade = null;
    }

    private IEnumerator Fade(float from, float to)
    {
        for (float t = 0f; t < fadeTime; t += Time.unscaledDeltaTime)
        {
            text.alpha = Mathf.Lerp(from, to, t / fadeTime);
            yield return null;
        }
        text.alpha = to;
    }

    private string TextAt(int index) => objectives != null ? objectives.TextAt(index) : "";
}
