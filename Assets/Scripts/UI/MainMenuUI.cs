using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles the main menu UI interactions and scene transitions.
/// </summary>
public class MainMenuUI : MonoBehaviour
{
    [Header("UI Buttons")]
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button quitButton;

    [Header("Settings")]
    [SerializeField] private string gameplaySceneName = "MainGame";

    private void Start()
    {
        // Setup button listeners
        if (startGameButton != null)
        {
            startGameButton.onClick.AddListener(OnStartGameClicked);
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(OnQuitClicked);
        }
    }

    private void OnDestroy()
    {
        // Cleanup listeners
        if (startGameButton != null)
        {
            startGameButton.onClick.RemoveListener(OnStartGameClicked);
        }

        if (quitButton != null)
        {
            quitButton.onClick.RemoveListener(OnQuitClicked);
        }
    }

    /// <summary>
    /// Called when the Start Game button is clicked.
    /// </summary>
    private void OnStartGameClicked()
    {
        // Use GameManager if available, otherwise load directly
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartGame();
        }
        else if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadMainGame();
        }
        else
        {
            SceneManager.LoadScene(gameplaySceneName);
        }
    }

    /// <summary>
    /// Called when the Quit button is clicked.
    /// </summary>
    private void OnQuitClicked()
    {
        // Delegate to GameManager if available for consistency
        if (GameManager.Instance != null)
        {
            GameManager.Instance.QuitGame();
        }
        else
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
