using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Utility for managing scene loading with transitions and loading screens.
/// </summary>
public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private float minimumLoadTime = 1f;
    [SerializeField] private string loadingSceneName = "LoadingScreen";

    [Header("Scene Names")]
    [SerializeField] private string mainMenuScene = "MainMenu";
    [SerializeField] private string characterCreationScene = "CharacterCreation";
    [SerializeField] private string mainGameScene = "MainGame";

    public event Action<string> OnSceneLoadStarted;
    public event Action<float> OnLoadProgress;
    public event Action<string> OnSceneLoadCompleted;

    private bool isLoading;
    private AsyncOperation currentLoadOperation;

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

    /// <summary>
    /// Loads a scene by name.
    /// </summary>
    public void LoadScene(string sceneName)
    {
        if (isLoading) return;
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    /// <summary>
    /// Loads the main menu scene.
    /// </summary>
    public void LoadMainMenu()
    {
        LoadScene(mainMenuScene);
    }

    /// <summary>
    /// Loads the character creation scene.
    /// </summary>
    public void LoadCharacterCreation()
    {
        LoadScene(characterCreationScene);
    }

    /// <summary>
    /// Loads the main game scene.
    /// </summary>
    public void LoadMainGame()
    {
        LoadScene(mainGameScene);
    }

    /// <summary>
    /// Reloads the current scene.
    /// </summary>
    public void ReloadCurrentScene()
    {
        LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Async scene loading with progress tracking.
    /// </summary>
    private IEnumerator LoadSceneAsync(string sceneName)
    {
        isLoading = true;
        OnSceneLoadStarted?.Invoke(sceneName);

        float startTime = Time.time;

        // Start loading the scene
        currentLoadOperation = SceneManager.LoadSceneAsync(sceneName);
        currentLoadOperation.allowSceneActivation = false;

        // Track progress
        while (!currentLoadOperation.isDone)
        {
            float progress = Mathf.Clamp01(currentLoadOperation.progress / 0.9f);
            OnLoadProgress?.Invoke(progress);

            // Scene is ready to activate at 0.9
            if (currentLoadOperation.progress >= 0.9f)
            {
                // Ensure minimum load time for smooth transitions
                float elapsedTime = Time.time - startTime;
                if (elapsedTime < minimumLoadTime)
                {
                    yield return new WaitForSeconds(minimumLoadTime - elapsedTime);
                }

                // Activate the scene
                currentLoadOperation.allowSceneActivation = true;
            }

            yield return null;
        }

        isLoading = false;
        OnSceneLoadCompleted?.Invoke(sceneName);
    }

    /// <summary>
    /// Loads a scene additively.
    /// </summary>
    public void LoadSceneAdditive(string sceneName)
    {
        StartCoroutine(LoadSceneAdditiveAsync(sceneName));
    }

    private IEnumerator LoadSceneAdditiveAsync(string sceneName)
    {
        var operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        while (!operation.isDone)
        {
            yield return null;
        }
    }

    /// <summary>
    /// Unloads a scene.
    /// </summary>
    public void UnloadScene(string sceneName)
    {
        SceneManager.UnloadSceneAsync(sceneName);
    }

    /// <summary>
    /// Gets the current scene name.
    /// </summary>
    public string GetCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

    /// <summary>
    /// Checks if currently loading.
    /// </summary>
    public bool IsLoading()
    {
        return isLoading;
    }

    /// <summary>
    /// Gets current load progress (0-1).
    /// </summary>
    public float GetLoadProgress()
    {
        if (currentLoadOperation == null) return 0f;
        return Mathf.Clamp01(currentLoadOperation.progress / 0.9f);
    }
}
