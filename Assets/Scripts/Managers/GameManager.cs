using UnityEngine;
using UnityEngine.SceneManagement;

namespace MercenaryWars.Managers
{
    /// <summary>
    /// Singleton game manager. Control game state and scenes.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        // Singleton instance
        public static GameManager Instance { get; private set; }

        [Header("Player Tracking")]
        [SerializeField] private int playerScore = 0;
        [SerializeField] private int playerHealth = 100;

        private void Awake()
        {
            // Enforce singleton pattern
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); // Keep alive between scenes
            }
            else
            {
                Destroy(gameObject); // Destroy duplicate
            }
        }

        /// <summary>
        /// Add points to score.
        /// </summary>
        public void AddScore(int amount)
        {
            playerScore += amount;
            Debug.Log($"Score updated: {playerScore}");
        }

        /// <summary>
        /// Change player health. Trigger Game Over if dead.
        /// </summary>
        public void UpdateHealth(int amount)
        {
            playerHealth += amount;
            
            if (playerHealth <= 0)
            {
                playerHealth = 0;
                TriggerGameOver();
            }
        }

        /// <summary>
        /// Load Level 1 scene.
        /// </summary>
        public void LoadLevel1()
        {
            ResetPlayerStats();
            SceneManager.LoadScene("Level1");
        }

        /// <summary>
        /// Load Level 2 scene.
        /// </summary>
        public void LoadLevel2()
        {
            SceneManager.LoadScene("Level2");
        }

        /// <summary>
        /// Load Main Menu scene.
        /// </summary>
        public void LoadMainMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }

        /// <summary>
        /// Load Game Over scene.
        /// </summary>
        public void LoadGameOver()
        {
            SceneManager.LoadScene("GameOver");
        }

        /// <summary>
        /// Handle game over logic.
        /// </summary>
        private void TriggerGameOver()
        {
            Debug.Log("Player dead. Game Over.");
            LoadGameOver();
        }

        /// <summary>
        /// Reset stats for new game.
        /// </summary>
        private void ResetPlayerStats()
        {
            playerScore = 0;
            playerHealth = 100;
        }
    }
}
