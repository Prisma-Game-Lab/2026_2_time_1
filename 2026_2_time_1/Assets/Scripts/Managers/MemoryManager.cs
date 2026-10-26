using UnityEngine;

public class MemoryManager : MonoBehaviour
{
    public static MemoryManager Current { get; private set; }

    public bool IsInMemory => GameManager.Instance != null && GameManager.Instance.IsInMemory;

    private void OnEnable()
    {
        Current = this;
    }

    private void OnDisable()
    {
        if (Current == this)
            Current = null;
    }

    public void EnterMemory(string sceneName)
    {
        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError($"[MemoryManager] Memory scene '{sceneName}' can't be loaded. Is it added to the Build Settings?");
            return;
        }

        GameManager.Instance.LoadMemoryScene(sceneName);
    }

    public void EndMemory()
    {
        if (!IsInMemory)
        {
            Debug.LogWarning("[MemoryManager] Not inside a memory, nothing to end.");
            return;
        }

        GameManager.Instance.ReturnFromMemory();
    }
}
