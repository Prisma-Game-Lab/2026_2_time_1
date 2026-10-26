using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private class FrozenScene
    {
        public Scene Scene;
        public readonly List<GameObject> DisabledRoots = new List<GameObject>();
    }

    public static GameManager Instance { get; private set; }

    private readonly Stack<FrozenScene> frozenScenes = new Stack<FrozenScene>();

    public bool IsTransitioning { get; private set; }
    public bool IsInMemory => frozenScenes.Count > 0;
    public int MemoryDepth => frozenScenes.Count;

    public event Action<string> OnMemoryEntered;
    public event Action<string> OnMemoryExited;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        if (Instance == null)
            new GameObject(nameof(GameManager)).AddComponent<GameManager>();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void LoadMemoryScene(string sceneName)
    {
        if (IsTransitioning)
            return;

        StartCoroutine(LoadMemoryRoutine(sceneName));
    }

    public void ReturnFromMemory()
    {
        if (!IsInMemory || IsTransitioning)
            return;

        StartCoroutine(ReturnFromMemoryRoutine());
    }

    private IEnumerator LoadMemoryRoutine(string sceneName)
    {
        IsTransitioning = true;

        FrozenScene frozen = new FrozenScene { Scene = SceneManager.GetActiveScene() };
        foreach (GameObject root in frozen.Scene.GetRootGameObjects())
        {
            if (!root.activeSelf)
                continue;
            root.SetActive(false);
            frozen.DisabledRoots.Add(root);
        }
        frozenScenes.Push(frozen);

        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));

        IsTransitioning = false;
        OnMemoryEntered?.Invoke(sceneName);
    }

    private IEnumerator ReturnFromMemoryRoutine()
    {
        IsTransitioning = true;

        Scene memoryScene = SceneManager.GetActiveScene();
        string memoryName = memoryScene.name;
        yield return SceneManager.UnloadSceneAsync(memoryScene);

        FrozenScene frozen = frozenScenes.Pop();
        foreach (GameObject root in frozen.DisabledRoots)
        {
            if (root != null)
                root.SetActive(true);
        }
        SceneManager.SetActiveScene(frozen.Scene);

        IsTransitioning = false;
        OnMemoryExited?.Invoke(memoryName);
    }
}
