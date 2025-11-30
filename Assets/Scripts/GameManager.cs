using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Singleton GameManager that handles game state transitions between MainMenu, Gameplay, and GameOver states.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    /// <summary>
    /// Defines the possible game states.
    /// </summary>
    public enum GameState
    {
        MainMenu,
        Gameplay,
        GameOver
    }

    [Header("Scene Names")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private string gameplaySceneName = "MainGame";

    /// <summary>
    /// Current game state.
    /// </summary>
    public GameState CurrentState { get; private set; } = GameState.MainMenu;

    /// <summary>
    /// Event triggered when the game state changes.
    /// </summary>
    public event System.Action<GameState> OnGameStateChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Transitions to the MainMenu state and loads the main menu scene.
    /// </summary>
    public void GoToMainMenu()
    {
        SetGameState(GameState.MainMenu);

        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadMainMenu();
        }
        else
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
    }

    /// <summary>
    /// Starts the game by transitioning to the Gameplay state and loading the main level.
    /// </summary>
    public void StartGame()
    {
        SetGameState(GameState.Gameplay);

        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadMainGame();
        }
        else
        {
            SceneManager.LoadScene(gameplaySceneName);
        }
    }

    /// <summary>
    /// Triggers the GameOver state.
    /// </summary>
    public void TriggerGameOver()
    {
        SetGameState(GameState.GameOver);
        // Game over logic can be handled by subscribers to OnGameStateChanged
    }

    /// <summary>
    /// Restarts the current game by reloading the gameplay scene.
    /// </summary>
    public void RestartGame()
    {
        SetGameState(GameState.Gameplay);

        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadMainGame();
        }
        else
        {
            SceneManager.LoadScene(gameplaySceneName);
        }
    }

    /// <summary>
    /// Sets the game state and triggers the state change event.
    /// </summary>
    private void SetGameState(GameState newState)
    {
        if (CurrentState != newState)
        {
            CurrentState = newState;
            OnGameStateChanged?.Invoke(newState);
            Debug.Log($"[GameManager] State changed to: {newState}");
        }
    }

    /// <summary>
    /// Quits the application.
    /// </summary>
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
