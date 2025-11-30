using UnityEngine;

/// <summary>
/// A collectible coin that destroys itself on collision with the player
/// and increments the player's score.
/// </summary>
public class Coin : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Points awarded when collecting this coin.")]
    [SerializeField] private int pointValue = 1;

    [Tooltip("Tag used to identify the player.")]
    [SerializeField] private string playerTag = "Player";

    [Header("Optional Effects")]
    [Tooltip("Sound effect to play when collected.")]
    [SerializeField] private string collectSoundName = "Collect";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            Collect(other.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(playerTag))
        {
            Collect(collision.gameObject);
        }
    }

    /// <summary>
    /// Handles the collection logic.
    /// </summary>
    private void Collect(GameObject player)
    {
        // Add score to player
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.AddScore(pointValue);
        }

        // Play collection sound if AudioManager exists
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(collectSoundName);
        }

        // Destroy the collectible
        Destroy(gameObject);
    }
}
